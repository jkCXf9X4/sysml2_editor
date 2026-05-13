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
  rm -rf "${temp_diff_root:-}" "${temp_commit_root:-}"
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

temp_diff_root="$(mktemp -d)"
temp_commit_root="$(mktemp -d)"

setup_git_repo() {
  local repo_root="$1"
  mkdir -p "${repo_root}/model"
  cp "${ROOT_DIR}/fixtures/tiny-single-file/model/root.sysml" "${repo_root}/model/root.sysml"
  git -C "${repo_root}" init -b main >/dev/null
  git -C "${repo_root}" config user.name "Sysml2Editor" >/dev/null
  git -C "${repo_root}" config user.email "sysml2editor@example.com" >/dev/null
}

setup_git_repo "${temp_diff_root}"
git -C "${temp_diff_root}" add . >/dev/null
git -C "${temp_diff_root}" commit -m "Initial commit" >/dev/null
git -C "${temp_diff_root}" checkout -b concept-ev >/dev/null
printf '\n  @Sysml2EditorIdentity { id = "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"; }\n  part def ThermalMonitor;\n' >> "${temp_diff_root}/model/root.sysml"
git -C "${temp_diff_root}" commit -am "Add ThermalMonitor" >/dev/null
git -C "${temp_diff_root}" checkout main >/dev/null

temp_git_status="$(curl -fsS "${BASE_URL}/api/workspaces/git/status?rootPath=${temp_diff_root}")"
temp_git_diff="$(curl -fsS "${BASE_URL}/api/workspaces/git/diff?rootPath=${temp_diff_root}&baseBranch=main&headBranch=concept-ev")"

setup_git_repo "${temp_commit_root}"
git -C "${temp_commit_root}" add . >/dev/null
git -C "${temp_commit_root}" commit -m "Initial commit" >/dev/null
printf '\n  @Sysml2EditorIdentity { id = "bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"; }\n  part def CoolingBar;\n' >> "${temp_commit_root}/model/root.sysml"
temp_commit_response="$(curl -fsS -X POST -H 'Content-Type: application/json' -d "{\"rootPath\":\"${temp_commit_root}\",\"summary\":\"Add CoolingBar part definition\"}" "${BASE_URL}/api/workspaces/git/commit")"
temp_commit_status="$(curl -fsS "${BASE_URL}/api/workspaces/git/status?rootPath=${temp_commit_root}")"

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

if [[ "${temp_git_status}" != *'"branch":"main"'* || "${temp_git_status}" != *'"isDirty":false'* ]]; then
  echo "Git status response did not report a clean main branch." >&2
  exit 1
fi

if [[ "${temp_git_diff}" != *'"name":"ThermalMonitor"'* || "${temp_git_diff}" != *'"relationship":"AddsModelItem"'* ]]; then
  echo "Git diff response did not include the expected branch comparison." >&2
  exit 1
fi

if [[ "${temp_commit_response}" != *'"succeeded":true'* || "${temp_commit_response}" != *'"commitSha"'* ]]; then
  echo "Git commit response did not report a successful commit." >&2
  exit 1
fi

if [[ "${temp_commit_status}" != *'"isDirty":false'* ]]; then
  echo "Git status after commit did not report a clean repository." >&2
  exit 1
fi

echo "backend-smoke: passed"
