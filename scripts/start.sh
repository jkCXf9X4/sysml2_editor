#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FRONTEND_DIR="$ROOT_DIR/src/frontend"
BACKEND_PROJECT="$ROOT_DIR/src/backend/Sysml2Editor.Api"
BACKEND_URL="${BACKEND_URL:-http://127.0.0.1:5087}"
BACKEND_HEALTH_URL="${BACKEND_URL%/}/api/health"
FRONTEND_URL="${FRONTEND_URL:-http://localhost:5173}"
FRONTEND_HEALTH_URL="${FRONTEND_HEALTH_URL:-http://127.0.0.1:5173}"
NUGET_HTTP_CACHE_PATH="${NUGET_HTTP_CACHE_PATH:-/tmp/sysml2_editor_nuget_http_cache}"
XDG_DATA_HOME="${XDG_DATA_HOME:-/tmp/sysml2_editor_xdg_data}"

backend_pid=""
frontend_pid=""

cleanup() {
  if [[ -n "$frontend_pid" ]] && kill -0 "$frontend_pid" 2>/dev/null; then
    kill "$frontend_pid" 2>/dev/null || true
  fi

  if [[ -n "$backend_pid" ]] && kill -0 "$backend_pid" 2>/dev/null; then
    kill "$backend_pid" 2>/dev/null || true
  fi
}

require_command() {
  local command_name="$1"

  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Missing required command: $command_name" >&2
    exit 1
  fi
}

trap cleanup EXIT INT TERM

require_command dotnet
require_command npm
require_command curl

mkdir -p "$NUGET_HTTP_CACHE_PATH"
mkdir -p "$XDG_DATA_HOME"

if [[ ! -d "$FRONTEND_DIR/node_modules" ]]; then
  echo "Installing frontend dependencies..."
  (cd "$FRONTEND_DIR" && npm install)
fi

if curl -fsS "$BACKEND_HEALTH_URL" >/dev/null 2>&1; then
  echo "Using existing backend at $BACKEND_URL"
else
  echo "Starting backend at $BACKEND_URL"
  XDG_DATA_HOME="$XDG_DATA_HOME" NUGET_HTTP_CACHE_PATH="$NUGET_HTTP_CACHE_PATH" dotnet run --project "$BACKEND_PROJECT" --urls "$BACKEND_URL" &
  backend_pid="$!"

  echo "Waiting for backend health at $BACKEND_HEALTH_URL"
  backend_ready=false
  for _ in {1..80}; do
    if curl -fsS "$BACKEND_HEALTH_URL" >/dev/null 2>&1; then
      backend_ready=true
      break
    fi

    if ! kill -0 "$backend_pid" 2>/dev/null; then
      echo "Backend exited before it became healthy." >&2
      exit 1
    fi

    sleep 0.25
  done

  if [[ "$backend_ready" != "true" ]]; then
    echo "Backend did not become healthy at $BACKEND_HEALTH_URL." >&2
    exit 1
  fi
fi

if curl -fsS "$FRONTEND_HEALTH_URL" >/dev/null 2>&1; then
  echo "Using existing frontend at $FRONTEND_URL"
else
  echo "Starting frontend dev server at $FRONTEND_URL"
  (cd "$FRONTEND_DIR" && npm run dev -- --host 127.0.0.1 --port 5173 --strictPort) &
  frontend_pid="$!"
fi

echo
echo "Application starting:"
echo "  Frontend: $FRONTEND_URL"
echo "  Backend:  $BACKEND_URL"
echo
echo "Press Ctrl-C to stop both processes."

if [[ -n "$backend_pid" && -n "$frontend_pid" ]]; then
  wait -n "$backend_pid" "$frontend_pid"
elif [[ -n "$backend_pid" ]]; then
  wait "$backend_pid"
elif [[ -n "$frontend_pid" ]]; then
  wait "$frontend_pid"
else
  echo "Both services were already running."
fi
