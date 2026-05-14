#!/usr/bin/env bash
set -euo pipefail

BACKEND_PORT="${SYSML2_EDITOR_BACKEND_PORT:-5087}"
FRONTEND_PORT="${SYSML2_EDITOR_FRONTEND_PORT:-5173}"
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

stopped_any=false

stop_matching_processes() {
  local label="$1"
  local pattern="$2"
  local pids

  pids="$(pgrep -f "$pattern" 2>/dev/null || true)"
  if [[ -z "$pids" ]]; then
    return
  fi

  echo "Stopping $label processes: $pids"
  stopped_any=true
  kill $pids 2>/dev/null || true
}

stop_port_listeners() {
  local label="$1"
  local port="$2"
  local pids

  if command -v fuser >/dev/null 2>&1; then
    pids="$(fuser -n tcp "$port" 2>/dev/null || true)"
    if [[ -n "$pids" ]]; then
      echo "Stopping $label listener on port $port: $pids"
      stopped_any=true
      kill $pids 2>/dev/null || true
    fi
    return
  fi

  if command -v lsof >/dev/null 2>&1; then
    pids="$(lsof -ti tcp:"$port" 2>/dev/null || true)"
    if [[ -n "$pids" ]]; then
      echo "Stopping $label listener on port $port: $pids"
      stopped_any=true
      kill $pids 2>/dev/null || true
    fi
  fi
}

stop_matching_processes "backend" "$ROOT_DIR/src/backend/Sysml2Editor.Api|Sysml2Editor.Api"
stop_matching_processes "frontend" "$ROOT_DIR/src/frontend|vite --host 127.0.0.1 --port 5173"
stop_port_listeners "backend" "$BACKEND_PORT"
stop_port_listeners "frontend" "$FRONTEND_PORT"

sleep 0.5

if [[ "$stopped_any" != "true" ]]; then
  echo "No sysml2_editor backend or frontend processes were found."
else
  echo "stop: requested shutdown for sysml2_editor backend/frontend processes."
fi
