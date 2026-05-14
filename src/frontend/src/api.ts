export type SourceRange = {
  startLine: number;
  startColumn: number;
  endLine: number;
  endColumn: number;
};

export type ModelContext = {
  workspaceId: string;
  repositoryId: string;
  repositoryAlias: string;
  rootPath: string;
  branch: string;
  commitSha: string | null;
  isWritable: boolean;
  writableReason: string;
};

export type ModelNode = {
  stableId: string;
  kind: string;
  name: string;
  qualifiedName: string;
  owningPackageId: string | null;
  sourceFile: string;
  sourceRange: SourceRange;
  attributes: Record<string, string>;
  modelStatus: string;
};

export type ModelEdge = {
  stableId: string;
  kind: string;
  sourceId: string;
  targetId: string | null;
  sourceFile: string;
  sourceRange: SourceRange;
  attributes: Record<string, string>;
  modelStatus: string;
};

export type ModelFile = {
  path: string;
  lineEnding: string;
  contentHash: string;
  role: string;
  isDirty: boolean;
};

export type TraceLink = {
  stableId: string;
  kind: string;
  sourceKind: string;
  sourceWorkspaceId: string;
  sourceId: string;
  targetKind: string;
  targetWorkspaceId: string;
  targetId: string | null;
  relationship: string;
  sourceFile: string;
  sourceRange: SourceRange;
  attributes: Record<string, string>;
};

export type Diagnostic = {
  severity: string;
  code: string;
  message: string;
  sourceFile: string;
  sourceRange: SourceRange;
};

export type OpaqueSpan = {
  sourceFile: string;
  sourceRange: SourceRange;
  text: string;
};

export type ModelGraph = {
  context: ModelContext;
  nodes: ModelNode[];
  edges: ModelEdge[];
  files: ModelFile[];
  traceLinks: TraceLink[];
  opaqueSpans: OpaqueSpan[];
  diagnostics: Diagnostic[];
};

export type SourceFile = {
  path: string;
  content: string;
  lineEnding: string;
  contentHash: string;
};

export type SavePreview = {
  changedFiles: string[];
  unchangedFiles: string[];
};

export type WriteResult = {
  succeeded: boolean;
  content: string | null;
  diagnostics: Diagnostic[];
};

export type ChangedModelItem = {
  stableId: string;
  kind: string;
  name: string;
  sourceFile: string;
};

export type ChangedFile = {
  path: string;
  status: string;
};

export type BranchDiff = {
  baseWorkspaceId: string;
  headWorkspaceId: string;
  baseBranch: string;
  headBranch: string;
  added: ChangedModelItem[];
  modified: ChangedModelItem[];
  removed: ChangedModelItem[];
  changedFiles: ChangedFile[];
  traceLinks: TraceLink[];
};

export type Projection = {
  workspaceId: string;
  nodeIds: string[];
  edgeIds: string[];
  filePaths: string[];
  traceLinkIds: string[];
  attributes: Record<string, string>;
};

export type MultiContextView = {
  viewId: string;
  kind: string;
  title: string;
  contexts: ModelContext[];
  graphs: ModelGraph[];
  projections: Projection[];
  crossContextTraceLinks: TraceLink[];
  diagnostics: Diagnostic[];
};

export type GitStatus = {
  repositoryRoot: string;
  branch: string;
  headSha: string;
  isDirty: boolean;
  changedFiles: { status: string; path: string }[];
};

export type GitCommitResult = {
  succeeded: boolean;
  commitSha: string | null;
  summary: string;
  diagnostics: Diagnostic[];
};

export type MergeConflictPreview = {
  baseBranch: string;
  headBranch: string;
  hasConflicts: boolean;
  conflictingFiles: string[];
  summary: string;
};

async function request<T>(url: string, options?: RequestInit): Promise<T | null> {
  try {
    const response = await fetch(url, {
      headers: { 'Content-Type': 'application/json' },
      ...options,
    });
    if (!response.ok) return null;
    return (await response.json()) as T;
  } catch {
    return null;
  }
}

export function checkHealth(): Promise<{ status: string } | null> {
  return request<{ status: string }>('/api/health');
}

export function fetchFixtureGraph(fixture: string, branch = 'main', writable = true): Promise<ModelGraph | null> {
  return request<ModelGraph>(`/api/fixtures/${fixture}/graph?branch=${branch}&writable=${writable}`);
}

export function fetchFixtureSource(fixture: string, path: string): Promise<SourceFile | null> {
  return request<SourceFile>(`/api/fixtures/${fixture}/source/${path}`);
}

export function previewFixtureRename(fixture: string, stableId: string, name: string): Promise<WriteResult | null> {
  return request<WriteResult>(`/api/fixtures/${fixture}/rename`, {
    method: 'POST',
    body: JSON.stringify({ stableId, name }),
  });
}

export function saveFixtureRename(fixture: string, stableId: string, name: string): Promise<WriteResult | null> {
  return request<WriteResult>(`/api/fixtures/${fixture}/rename/save`, {
    method: 'POST',
    body: JSON.stringify({ stableId, name }),
  });
}

export function createFixtureElement(
  fixture: string,
  sourceFile: string,
  kind: string,
  name: string,
  typeName?: string,
): Promise<WriteResult | null> {
  return request<WriteResult>(`/api/fixtures/${fixture}/create-element`, {
    method: 'POST',
    body: JSON.stringify({ sourceFile, kind, name, typeName }),
  });
}

export function deleteFixtureElement(fixture: string, stableId: string): Promise<WriteResult | null> {
  return request<WriteResult>(`/api/fixtures/${fixture}/delete-element`, {
    method: 'POST',
    body: JSON.stringify({ stableId }),
  });
}

export function saveFixtureDraft(
  fixture: string,
  sourceFile: string,
  kind: string,
  name: string,
  typeName?: string,
): Promise<WriteResult | null> {
  return request<WriteResult>(`/api/fixtures/${fixture}/save-draft`, {
    method: 'POST',
    body: JSON.stringify({ sourceFile, kind, name, typeName }),
  });
}

export function fetchBranchDivergenceDiff(): Promise<BranchDiff | null> {
  return request<BranchDiff>('/api/fixtures/branch-divergence/diff');
}

export function fetchBranchDivergenceMultiContextView(): Promise<MultiContextView | null> {
  return request<MultiContextView>('/api/fixtures/branch-divergence/multi-context-view');
}

export function fetchGitGraph(rootPath: string, branch = 'main', writable = true): Promise<ModelGraph | null> {
  return request<ModelGraph>(`/api/workspaces/git/graph?rootPath=${encodeURIComponent(rootPath)}&branch=${branch}&writable=${writable}`);
}

export function fetchGitSource(rootPath: string, branch: string, path: string): Promise<SourceFile | null> {
  return request<SourceFile>(
    `/api/workspaces/git/source?rootPath=${encodeURIComponent(rootPath)}&branch=${branch}&path=${encodeURIComponent(path)}`,
  );
}

export function fetchGitStatus(rootPath: string): Promise<GitStatus | null> {
  return request<GitStatus>(`/api/workspaces/git/status?rootPath=${encodeURIComponent(rootPath)}`);
}

export function commitGitWorkspace(rootPath: string, summary: string): Promise<GitCommitResult | null> {
  return request<GitCommitResult>('/api/workspaces/git/commit', {
    method: 'POST',
    body: JSON.stringify({ rootPath, summary }),
  });
}

export function fetchGitDiff(rootPath: string, baseBranch: string, headBranch: string): Promise<BranchDiff | null> {
  return request<BranchDiff>(
    `/api/workspaces/git/diff?rootPath=${encodeURIComponent(rootPath)}&baseBranch=${baseBranch}&headBranch=${headBranch}`,
  );
}

export function fetchGitMergePreview(rootPath: string, baseBranch: string, headBranch: string): Promise<MergeConflictPreview | null> {
  return request<MergeConflictPreview>(
    `/api/workspaces/git/merge-preview?rootPath=${encodeURIComponent(rootPath)}&baseBranch=${baseBranch}&headBranch=${headBranch}`,
  );
}

// --- Phase 3 workspace context management ---

export function openRepository(path: string, branch?: string): Promise<OpenRepositoryResponse | null> {
  return request<OpenRepositoryResponse>('/api/repositories/open', {
    method: 'POST',
    body: JSON.stringify({ path, branch }),
  });
}

export function listWorkspaceContexts(): Promise<WorkspaceList | null> {
  return request<WorkspaceList>('/api/workspace-contexts');
}

export function closeWorkspaceContext(workspaceId: string): Promise<{ workspaceId: string; closed: boolean } | null> {
  return request<{ workspaceId: string; closed: boolean }>(`/api/workspace-contexts/${workspaceId}`, {
    method: 'DELETE',
  });
}

export function getWorkspaceGraph(workspaceId: string): Promise<ModelGraph | null> {
  return request<ModelGraph>(`/api/workspace-contexts/${workspaceId}/model`);
}

export function getWorkspaceSourceFile(workspaceId: string, path: string): Promise<SourceFile | null> {
  return request<SourceFile>(`/api/workspace-contexts/${workspaceId}/files?path=${encodeURIComponent(path)}`);
}

export function createWorktree(repositoryId: string, branch: string, path: string, createBranch = false): Promise<WorktreeResponse | null> {
  return request<WorktreeResponse>('/api/workspace-contexts/worktrees', {
    method: 'POST',
    body: JSON.stringify({ repositoryId, branch, path, createBranch }),
  });
}

export function commitWorkspaceContext(workspaceId: string, summary: string): Promise<GitCommitResult | null> {
  return request<GitCommitResult>(`/api/workspace-contexts/${workspaceId}/commit`, {
    method: 'POST',
    body: JSON.stringify({ summary }),
  });
}

// --- Phase 4 saved views ---

export function createSavedView(payload: SavedViewCreate): Promise<SavedView | null> {
  return request<SavedView>('/api/saved-views', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export function listSavedViews(): Promise<SavedViewList | null> {
  return request<SavedViewList>('/api/saved-views');
}

export function getSavedView(viewId: string): Promise<SavedView | null> {
  return request<SavedView>(`/api/saved-views/${viewId}`);
}

export function updateSavedView(viewId: string, payload: SavedViewUpdate): Promise<SavedView | null> {
  return request<SavedView>(`/api/saved-views/${viewId}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export function deleteSavedView(viewId: string): Promise<{ viewId: string; deleted: boolean } | null> {
  return request<{ viewId: string; deleted: boolean }>(`/api/saved-views/${viewId}`, {
    method: 'DELETE',
  });
}

export function getTraceMatrix(workspaceId: string): Promise<TraceMatrix | null> {
  return request<TraceMatrix>(`/api/workspace-contexts/${workspaceId}/trace-matrix`);
}

// --- Phase 3-4 types ---

export type WorkspaceContext = {
  workspaceId: string;
  repositoryId: string;
  repositoryAlias: string;
  rootPath: string;
  branch: string;
  commitSha: string | null;
  isWritable: boolean;
  writableReason: string;
  openedAt: string;
};

export type OpenRepositoryResponse = {
  repositoryId: string;
  workspaceId: string;
  rootPath: string;
  branch: string;
  isWritable: boolean;
  graph: ModelGraph;
};

export type WorkspaceList = {
  contexts: WorkspaceContext[];
};

export type WorktreeResponse = {
  workspaceId: string;
  repositoryId: string;
  rootPath: string;
  branch: string;
  isWritable: boolean;
  writableReason: string;
  diagnostics: Diagnostic[];
};

export type SavedView = {
  viewId: string;
  name: string;
  kind: string;
  workspaceId: string | null;
  repositoryId: string | null;
  branch: string | null;
  includedNodeIds: string[];
  excludedNodeIds: string[];
  filters: Record<string, string>;
  attributes: Record<string, string>;
  storageMode: string;
  createdAt: string;
  updatedAt: string;
};

export type SavedViewList = {
  views: SavedView[];
};

export type SavedViewCreate = {
  name: string;
  kind: string;
  workspaceId?: string;
  repositoryId?: string;
  branch?: string;
  includedNodeIds?: string[];
  excludedNodeIds?: string[];
  filters?: Record<string, string>;
  attributes?: Record<string, string>;
  storageMode: string;
};

export type SavedViewUpdate = {
  name?: string;
  includedNodeIds?: string[];
  excludedNodeIds?: string[];
  filters?: Record<string, string>;
  attributes?: Record<string, string>;
};

export type TraceMatrixCell = {
  sourceId: string;
  targetId: string;
  relationship: string;
  traceLinkId: string | null;
  sourceFile: string;
  isDirect: boolean;
};

export type TraceMatrix = {
  viewId: string;
  kind: string;
  title: string;
  workspaceId: string;
  sourceNodeIds: string[];
  targetNodeIds: string[];
  cells: TraceMatrixCell[];
};
