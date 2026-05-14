#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PORT="${SYSML2_EDITOR_BROWSER_PORT:-5174}"
BASE_URL="http://127.0.0.1:${PORT}"
LOG_FILE="$(mktemp)"
DOM_FILE="$(mktemp)"
CHROME_LOG_FILE="$(mktemp)"
CHROME_PROFILE_DIR="$(mktemp -d)"

cleanup() {
  if [[ -n "${SERVER_PID:-}" ]] && kill -0 "${SERVER_PID}" 2>/dev/null; then
    kill "${SERVER_PID}" 2>/dev/null || true
    wait "${SERVER_PID}" 2>/dev/null || true
  fi

  rm -f "${LOG_FILE}" "${DOM_FILE}" "${CHROME_LOG_FILE}"
  rm -rf "${CHROME_PROFILE_DIR}"
}

find_chrome() {
  if command -v google-chrome >/dev/null 2>&1; then
    command -v google-chrome
    return
  fi

  if command -v chromium >/dev/null 2>&1; then
    command -v chromium
    return
  fi

  if command -v chromium-browser >/dev/null 2>&1; then
    command -v chromium-browser
    return
  fi

  echo "Missing required browser command: google-chrome, chromium, or chromium-browser" >&2
  exit 1
}

trap cleanup EXIT

CHROME_BIN="$(find_chrome)"

(cd "${ROOT_DIR}/src/frontend" && npm run dev -- --host 127.0.0.1 --port "${PORT}") >"${LOG_FILE}" 2>&1 &
SERVER_PID="$!"

SERVER_READY=false
for _ in {1..40}; do
  if curl -fsS "${BASE_URL}" >/dev/null 2>&1; then
    SERVER_READY=true
    break
  fi

  if ! kill -0 "${SERVER_PID}" 2>/dev/null; then
    cat "${LOG_FILE}" >&2
    exit 1
  fi

  sleep 0.25
done

if [[ "${SERVER_READY}" != "true" ]]; then
  echo "Frontend dev server did not become ready at ${BASE_URL}." >&2
  cat "${LOG_FILE}" >&2
  exit 1
fi

"${CHROME_BIN}" \
  --headless=new \
  --disable-gpu \
  --no-sandbox \
  --disable-dev-shm-usage \
  --user-data-dir="${CHROME_PROFILE_DIR}" \
  --virtual-time-budget=5000 \
  --dump-dom \
  "${BASE_URL}" >"${DOM_FILE}" 2>"${CHROME_LOG_FILE}"

if ! grep -q 'sysml2_editor' "${DOM_FILE}"; then
  echo "Browser smoke DOM did not include the application brand." >&2
  cat "${CHROME_LOG_FILE}" >&2
  cat "${DOM_FILE}" >&2
  exit 1
fi

if ! grep -q 'Workbench layout' "${DOM_FILE}"; then
  echo "Browser smoke DOM did not include the workbench layout." >&2
  cat "${CHROME_LOG_FILE}" >&2
  cat "${DOM_FILE}" >&2
  exit 1
fi

echo "frontend-browser-smoke: passed"
