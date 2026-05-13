#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PORT="${SYSML2_EDITOR_FRONTEND_PORT:-5173}"
BASE_URL="http://127.0.0.1:${PORT}"
LOG_FILE="$(mktemp)"

cleanup() {
  if [[ -n "${SERVER_PID:-}" ]] && kill -0 "${SERVER_PID}" 2>/dev/null; then
    kill "${SERVER_PID}" 2>/dev/null || true
    wait "${SERVER_PID}" 2>/dev/null || true
  fi
  rm -f "${LOG_FILE}"
}

trap cleanup EXIT

(cd "${ROOT_DIR}/src/frontend" && npm run dev -- --host 127.0.0.1 --port "${PORT}") >"${LOG_FILE}" 2>&1 &
SERVER_PID="$!"

for _ in {1..40}; do
  if curl -fsS "${BASE_URL}" >/dev/null 2>&1; then
    break
  fi

  if ! kill -0 "${SERVER_PID}" 2>/dev/null; then
    cat "${LOG_FILE}" >&2
    exit 1
  fi

  sleep 0.25
done

html="$(curl -fsS "${BASE_URL}")"
if [[ "${html}" != *'<div id="root">'* ]]; then
  echo "Frontend smoke response did not include the React root." >&2
  exit 1
fi

echo "frontend-smoke: passed"
