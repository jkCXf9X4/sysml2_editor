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
public sealed record GitCommitRequest(string RootPath, string Summary);
