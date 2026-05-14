using Microsoft.OpenApi.Models;
using Sysml2Editor.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<SysmlMvpService>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sysml2Editor API",
        Version = "v1"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend-dev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend-dev");

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.MapGet("/api/fixtures/{fixture}/graph", (string fixture, string? branch, bool? writable, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    return Directory.Exists(root)
        ? Results.Ok(service.ParseRepository(root, branch ?? "main", writable ?? true))
        : Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
})
    .WithName("GetFixtureGraph");

app.MapGet("/api/fixtures/{fixture}/source/{**path}", (string fixture, string path, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    var fullPath = Path.GetFullPath(Path.Combine(root, path));
    if (!fullPath.StartsWith(Path.GetFullPath(root), StringComparison.Ordinal) || !System.IO.File.Exists(fullPath))
    {
        return Results.NotFound(new { message = $"Source file '{path}' was not found." });
    }

    return Results.Ok(service.GetSourceFile(root, path));
})
    .WithName("GetFixtureSourceFile");

app.MapPost("/api/fixtures/{fixture}/rename", (string fixture, RenameRequest request, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    if (!Directory.Exists(root))
    {
        return Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
    }

    var graph = service.ParseRepository(root);
    var result = service.RenameElement(graph, request.StableId, request.Name);
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
})
    .WithName("PreviewFixtureRename");

app.MapPost("/api/fixtures/{fixture}/rename/save", (string fixture, RenameRequest request, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    if (!Directory.Exists(root))
    {
        return Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
    }

    var result = service.SaveRename(root, request.StableId, request.Name);
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
})
    .WithName("SaveFixtureRename");

app.MapPost("/api/fixtures/{fixture}/create-element", (string fixture, CreateElementRequest request, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    if (!Directory.Exists(root))
    {
        return Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
    }

    var result = service.CreateElement(root, request.SourceFile, request.Kind, request.Name, request.TypeName);
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
})
    .WithName("CreateFixtureElement");

app.MapPost("/api/fixtures/{fixture}/delete-element", (string fixture, DeleteElementRequest request, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    if (!Directory.Exists(root))
    {
        return Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
    }

    var result = service.DeleteElement(root, request.StableId);
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
})
    .WithName("DeleteFixtureElement");

app.MapPost("/api/fixtures/{fixture}/save-draft", (string fixture, SaveDraftRequest request, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot(fixture);
    if (!Directory.Exists(root))
    {
        return Results.NotFound(new { message = $"Fixture '{fixture}' was not found." });
    }

    var sourceFile = request.SourceFile;
    var result = service.CreateElement(root, sourceFile, request.Kind, request.Name, request.TypeName);
    return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);
})
    .WithName("SaveFixtureDraft");

app.MapGet("/api/fixtures/multi-file-modular/save-preview/rename", (string stableId, string name, SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot("multi-file-modular");
    return Results.Ok(service.PreviewRenameTouchesOnlyOwner(root, stableId, name));
})
    .WithName("PreviewFixtureRenameChangedFiles");

app.MapGet("/api/fixtures/branch-divergence/diff", (SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot("branch-divergence");
    return Results.Ok(service.CompareBranches(Path.Combine(root, "base"), Path.Combine(root, "head")));
})
    .WithName("GetBranchDivergenceDiff");

app.MapGet("/api/fixtures/branch-divergence/multi-context-view", (SysmlMvpService service) =>
{
    var root = ResolveFixtureRoot("branch-divergence");
    return Results.Ok(service.BuildMultiContextView(Path.Combine(root, "base"), Path.Combine(root, "head")));
})
    .WithName("GetBranchDivergenceMultiContextView");

app.MapGet("/api/workspaces/git/graph", (string rootPath, string branch, bool? writable, SysmlMvpService service) =>
{
    return Directory.Exists(rootPath)
        ? Results.Ok(service.ParseGitRepository(rootPath, branch, writable ?? true))
        : Results.NotFound(new { message = $"Repository root '{rootPath}' was not found." });
})
    .WithName("GetGitWorkspaceGraph");

app.MapGet("/api/workspaces/git/source", (string rootPath, string branch, string path, SysmlMvpService service) =>
{
    return Directory.Exists(rootPath)
        ? Results.Ok(service.GetGitSourceFile(rootPath, branch, path))
        : Results.NotFound(new { message = $"Repository root '{rootPath}' was not found." });
})
    .WithName("GetGitWorkspaceSourceFile");

app.MapGet("/api/workspaces/git/status", (string rootPath, SysmlMvpService service) =>
{
    return Directory.Exists(rootPath)
        ? Results.Ok(service.GetGitStatus(rootPath))
        : Results.NotFound(new { message = $"Repository root '{rootPath}' was not found." });
})
    .WithName("GetGitWorkspaceStatus");

app.MapPost("/api/workspaces/git/commit", (GitCommitRequest request, SysmlMvpService service) =>
{
    return Directory.Exists(request.RootPath)
        ? Results.Ok(service.CommitAll(request.RootPath, request.Summary))
        : Results.NotFound(new { message = $"Repository root '{request.RootPath}' was not found." });
})
    .WithName("CommitGitWorkspace");

app.MapGet("/api/workspaces/git/diff", (string rootPath, string baseBranch, string headBranch, SysmlMvpService service) =>
{
    return Directory.Exists(rootPath)
        ? Results.Ok(service.CompareGitBranches(rootPath, baseBranch, headBranch))
        : Results.NotFound(new { message = $"Repository root '{rootPath}' was not found." });
})
    .WithName("CompareGitWorkspaceBranches");

app.MapGet("/api/workspaces/git/merge-preview", (string rootPath, string baseBranch, string headBranch, SysmlMvpService service) =>
{
    return Directory.Exists(rootPath)
        ? Results.Ok(service.PreviewMergeConflict(rootPath, baseBranch, headBranch))
        : Results.NotFound(new { message = $"Repository root '{rootPath}' was not found." });
})
    .WithName("PreviewGitMergeConflict");

app.MapPost("/api/repositories/open", (OpenRepoRequest request, SysmlMvpService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Path) || !Path.IsPathRooted(request.Path))
        return Results.BadRequest(new { code = "InvalidPath", message = "Path must be absolute and non-empty." });
    if (!Directory.Exists(request.Path))
        return Results.NotFound(new { code = "RepositoryNotFound", message = $"Repository path '{request.Path}' does not exist." });
    try
    {
        return Results.Ok(service.OpenRepository(request.Path, request.Branch));
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(new { code = "RepositoryNotUsable", message = ex.Message });
    }
})
    .WithName("OpenRepository");

app.MapGet("/api/workspace-contexts", (SysmlMvpService service) =>
{
    return Results.Ok(service.ListWorkspaceContexts());
})
    .WithName("ListWorkspaceContexts");

app.MapDelete("/api/workspace-contexts/{workspaceId}", (string workspaceId, SysmlMvpService service) =>
{
    return service.CloseWorkspaceContext(workspaceId)
        ? Results.Ok(new { workspaceId, closed = true })
        : Results.NotFound(new { code = "WorkspaceNotFound", message = $"Workspace '{workspaceId}' not found." });
})
    .WithName("CloseWorkspaceContext");

app.MapGet("/api/workspace-contexts/{workspaceId}/model", (string workspaceId, SysmlMvpService service) =>
{
    var graph = service.GetWorkspaceGraph(workspaceId);
    return graph is not null
        ? Results.Ok(graph)
        : Results.NotFound(new { code = "WorkspaceNotFound", message = $"Workspace '{workspaceId}' not found." });
})
    .WithName("GetWorkspaceModelGraph");

app.MapGet("/api/workspace-contexts/{workspaceId}/files", (string workspaceId, string path, SysmlMvpService service) =>
{
    var source = service.GetWorkspaceSourceFile(workspaceId, path);
    return source is not null
        ? Results.Ok(source)
        : Results.NotFound(new { code = "FileNotFound", message = $"File '{path}' not found in workspace '{workspaceId}'." });
})
    .WithName("GetWorkspaceSourceFile");

app.MapPost("/api/workspace-contexts/worktrees", (WorktreeRequestDto request, SysmlMvpService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Path) || !Path.IsPathRooted(request.Path))
        return Results.BadRequest(new { code = "InvalidPath", message = "Worktree path must be absolute and non-empty." });
    if (string.IsNullOrWhiteSpace(request.Branch))
        return Results.BadRequest(new { code = "InvalidBranch", message = "Branch name is required." });
    var result = service.CreateWorktree(request.RepositoryId, request.Branch, request.Path, request.CreateBranch);
    return result.IsWritable
        ? Results.Ok(result)
        : Results.Conflict(result);
})
    .WithName("CreateWorktree");

app.MapPost("/api/workspace-contexts/{workspaceId}/commit", (string workspaceId, CommitRequest request, SysmlMvpService service) =>
{
    var ctx = service.GetWorkspaceContext(workspaceId);
    if (ctx is null)
        return Results.NotFound(new { code = "WorkspaceNotFound", message = $"Workspace '{workspaceId}' not found." });
    if (!ctx.IsWritable)
        return Results.Conflict(new { code = "ContextNotWritable", message = "Cannot commit from a read-only context." });
    return Results.Ok(service.CommitAll(ctx.RootPath, request.Summary));
})
    .WithName("CommitWorkspaceContext");

app.MapPost("/api/saved-views", (SavedViewCreateDto request, SysmlMvpService service) =>
{
    return Results.Ok(service.CreateSavedView(request));
})
    .WithName("CreateSavedView");

app.MapGet("/api/saved-views", (SysmlMvpService service) =>
{
    return Results.Ok(service.ListSavedViews());
})
    .WithName("ListSavedViews");

app.MapGet("/api/saved-views/{viewId}", (string viewId, SysmlMvpService service) =>
{
    var view = service.GetSavedView(viewId);
    return view is not null
        ? Results.Ok(view)
        : Results.NotFound(new { code = "ViewNotFound", message = $"Saved view '{viewId}' not found." });
})
    .WithName("GetSavedView");

app.MapPut("/api/saved-views/{viewId}", (string viewId, SavedViewUpdateDto request, SysmlMvpService service) =>
{
    var updated = service.UpdateSavedView(viewId, request);
    return updated is not null
        ? Results.Ok(updated)
        : Results.NotFound(new { code = "ViewNotFound", message = $"Saved view '{viewId}' not found." });
})
    .WithName("UpdateSavedView");

app.MapDelete("/api/saved-views/{viewId}", (string viewId, SysmlMvpService service) =>
{
    return service.DeleteSavedView(viewId)
        ? Results.Ok(new { viewId, deleted = true })
        : Results.NotFound(new { code = "ViewNotFound", message = $"Saved view '{viewId}' not found." });
})
    .WithName("DeleteSavedView");

app.MapGet("/api/workspace-contexts/{workspaceId}/trace-matrix", (string workspaceId, SysmlMvpService service) =>
{
    return Results.Ok(service.BuildTraceMatrix(workspaceId));
})
    .WithName("GetTraceMatrix");

app.Run();

static string ResolveFixtureRoot(string fixture)
{
    var root = FindRepositoryRoot();
    return Path.Combine(root, "fixtures", fixture);
}

static string FindRepositoryRoot()
{
    var current = AppContext.BaseDirectory;
    while (current is not null)
    {
        if (Directory.Exists(Path.Combine(current, "fixtures")) && Directory.Exists(Path.Combine(current, "src")))
        {
            return current;
        }

        current = Directory.GetParent(current)?.FullName;
    }

    throw new InvalidOperationException("Repository root was not found.");
}

public sealed record RenameRequest(string StableId, string Name);
public sealed record CreateElementRequest(string SourceFile, string Kind, string Name, string? TypeName);
public sealed record DeleteElementRequest(string StableId);
public sealed record SaveDraftRequest(string SourceFile, string Kind, string Name, string? TypeName, string? AfterStableId);
public sealed record GitCommitRequest(string RootPath, string Summary);
public sealed record OpenRepoRequest(string Path, string? Branch);
public sealed record CommitRequest(string Summary);
