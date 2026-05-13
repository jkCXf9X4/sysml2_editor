#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PORT="${SYSML2_EDITOR_BACKEND_PORT:-5087}"
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

dotnet run --project "${ROOT_DIR}/src/backend/Sysml2Editor.Api" --urls "${BASE_URL}" >"${LOG_FILE}" 2>&1 &
SERVER_PID="$!"

for _ in {1..40}; do
  if curl -fsS "${BASE_URL}/api/health" >/dev/null 2>&1; then
    break
  fi

  if ! kill -0 "${SERVER_PID}" 2>/dev/null; then
    cat "${LOG_FILE}" >&2
    exit 1
  fi

  sleep 0.25
done

health_response="$(curl -fsS "${BASE_URL}/api/health")"
openapi_response="$(curl -fsS "${BASE_URL}/swagger/v1/swagger.json")"

if [[ "${health_response}" != '{"status":"ok"}' ]]; then
  echo "Unexpected health response: ${health_response}" >&2
  exit 1
fi

if [[ "${openapi_response}" != *'"title": "Sysml2Editor API"'* ]]; then
  echo "OpenAPI document did not include expected API title." >&2
  exit 1
fi

echo "backend-smoke: passed"
