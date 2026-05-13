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
graph_response="$(curl -fsS "${BASE_URL}/api/fixtures/tiny-single-file/graph")"
source_response="$(curl -fsS "${BASE_URL}/api/fixtures/tiny-single-file/source/model/root.sysml")"
diff_response="$(curl -fsS "${BASE_URL}/api/fixtures/branch-divergence/diff")"
multi_context_response="$(curl -fsS "${BASE_URL}/api/fixtures/branch-divergence/multi-context-view")"

if [[ "${health_response}" != '{"status":"ok"}' ]]; then
  echo "Unexpected health response: ${health_response}" >&2
  exit 1
fi

if [[ "${openapi_response}" != *'"title": "Sysml2Editor API"'* ]]; then
  echo "OpenAPI document did not include expected API title." >&2
  exit 1
fi

if [[ "${graph_response}" != *'"workspaceId":"workspace-tiny-single-file-main"'* ]]; then
  echo "Graph response did not include expected workspace context." >&2
  exit 1
fi

if [[ "${source_response}" != *'"contentHash":"sha256:06a2af0f2cc469f13218466ffbb6a11e83b6be0bc2884a02ef929890681a9645"'* ]]; then
  echo "Source response did not preserve expected content hash." >&2
  exit 1
fi

if [[ "${diff_response}" != *'"relationship":"AddsModelItem"'* ]]; then
  echo "Diff response did not include expected branch trace relationship." >&2
  exit 1
fi

if [[ "${multi_context_response}" != *'"viewId":"view-branch-divergence-base-head"'* ]]; then
  echo "Multi-context response did not include expected view ID." >&2
  exit 1
fi

echo "backend-smoke: passed"
