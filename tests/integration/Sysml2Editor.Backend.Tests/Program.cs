using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using Sysml2Editor.Domain;

var root = FindRepositoryRoot();
var fixtures = Path.Combine(root, "fixtures");
var service = new SysmlMvpService();
var failures = new List<string>();
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

Run("parse_minimal_graph", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "tiny-single-file"));
    AssertJsonEquals(ReadFixture("tiny-single-file/expected/graph.json"), ToJson(graph));
});

Run("model_graph_has_context", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "tiny-single-file"));
    AssertEqual("workspace-tiny-single-file-main", graph.Context.WorkspaceId);
    AssertEqual("fixture-tiny-single-file", graph.Context.RepositoryId);
    AssertEqual("main", graph.Context.Branch);
    AssertTrue(graph.Context.IsWritable);
});

Run("derive_item_to_file_traceability", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "tiny-single-file"));
    var structuredNodeIds = graph.Nodes.Select(node => node.StableId).ToHashSet(StringComparer.Ordinal);
    var itemToFileIds = graph.TraceLinks
        .Where(link => link.Kind == "ItemToFile")
        .Select(link => link.SourceId)
        .ToHashSet(StringComparer.Ordinal);

    AssertTrue(structuredNodeIds.SetEquals(itemToFileIds));
});

Run("derive_import_traceability", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "multi-file-modular"), "main", true);
    AssertJsonEquals(ReadFixture("multi-file-modular/expected/trace-links.json"), ToJson(graph.TraceLinks));
});

Run("malformed_input_reports_diagnostic", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "invalid-input"));
    AssertJsonEquals(ReadFixture("invalid-input/expected/diagnostics.json"), ToJson(graph.Diagnostics));
});

Run("get_source_file_preserves_text", () =>
{
    var source = service.GetSourceFile(Path.Combine(fixtures, "tiny-single-file"), "model/root.sysml");
    var original = File.ReadAllText(Path.Combine(fixtures, "tiny-single-file/model/root.sysml"));
    AssertEqual(original, source.Content);
    AssertEqual("LF", source.LineEnding);
    AssertEqual("sha256:06a2af0f2cc469f13218466ffbb6a11e83b6be0bc2884a02ef929890681a9645", source.ContentHash);
});

Run("parse_round_trip_minimal", () =>
{
    var firstGraph = service.ParseRepository(Path.Combine(fixtures, "tiny-single-file"));
    var written = service.Write(firstGraph);
    var tempRoot = Path.Combine(Path.GetTempPath(), $"sysml2-editor-roundtrip-{Guid.NewGuid():N}");
    Directory.CreateDirectory(Path.Combine(tempRoot, "model"));
    try
    {
        File.WriteAllText(Path.Combine(tempRoot, "model/root.sysml"), written);
        var reparsed = service.ParseRepository(tempRoot, firstGraph.Context);
        AssertJsonEquals(ReadFixture("tiny-single-file/expected/graph.json"), ToJson(reparsed));
    }
    finally
    {
        Directory.Delete(tempRoot, recursive: true);
    }
});

Run("save_touches_only_owner", () =>
{
    var tempRoot = CopyFixtureToTemp("multi-file-modular");
    try
    {
        var powerBefore = File.ReadAllText(Path.Combine(tempRoot, "model/power.sysml"));
        var result = service.SaveRename(tempRoot, "33333333-3333-4333-8333-333333333333", "tractionBattery");
        AssertTrue(result.Succeeded);
        AssertTrue(File.ReadAllText(Path.Combine(tempRoot, "model/root.sysml")).Contains("part tractionBattery : Power::BatteryPack;", StringComparison.Ordinal));
        AssertEqual(powerBefore, File.ReadAllText(Path.Combine(tempRoot, "model/power.sysml")));

        var preview = service.PreviewRenameTouchesOnlyOwner(tempRoot, "33333333-3333-4333-8333-333333333333", "tractionBattery");
        var expected = JsonNode.Parse(ReadFixture("multi-file-modular/expected/changed-files.json"))!["renameBatteryUsageToTractionBattery"]!.ToJsonString();
        AssertJsonEquals(expected, ToJson(preview));
    }
    finally
    {
        Directory.Delete(tempRoot, recursive: true);
    }
});

Run("stable_id_survives_rename", () =>
{
    var graph = service.ParseRepository(Path.Combine(fixtures, "tiny-single-file"));
    var result = service.RenameElement(graph, "22222222-2222-4222-8222-222222222222", "tractionBattery");
    AssertTrue(result.Succeeded);
    AssertTrue(result.Content!.Contains("""@Sysml2EditorIdentity { id = "22222222-2222-4222-8222-222222222222"; }""", StringComparison.Ordinal));
    AssertTrue(result.Content.Contains("part tractionBattery : BatteryPack;", StringComparison.Ordinal));
});

Run("missing_id_blocks_write_until_backfill", () =>
{
    var tempRoot = Path.Combine(Path.GetTempPath(), $"sysml2-editor-missing-id-{Guid.NewGuid():N}");
    Directory.CreateDirectory(Path.Combine(tempRoot, "model"));
    try
    {
        File.WriteAllText(Path.Combine(tempRoot, "model/root.sysml"), "package Vehicle {\n  part def BatteryPack;\n}\n");
        var graph = service.ParseRepository(tempRoot);
        AssertTrue(graph.Diagnostics.Any(diagnostic => diagnostic.Code == "MissingStableId"));
        var result = service.RenameElement(graph, graph.Nodes.First(node => node.Kind == "PartDefinition").StableId, "Pack");
        AssertTrue(!result.Succeeded);
        AssertTrue(result.Diagnostics.Any(diagnostic => diagnostic.Code == "MissingStableIdBlocksWrite"));
    }
    finally
    {
        Directory.Delete(tempRoot, recursive: true);
    }
});

Run("create_and_delete_supported_element", () =>
{
    var tempRoot = CopyFixtureToTemp("phase-2-editing");
    try
    {
        var create = service.CreateElement(tempRoot, "model/BatterySystem.sysml", "PartDefinition", "ThermalBarrier");
        AssertTrue(create.Succeeded);
        var createdGraph = service.ParseRepository(tempRoot);
        AssertTrue(createdGraph.Nodes.Any(node => node.Name == "ThermalBarrier" && node.Kind == "PartDefinition"));

        var createdNode = createdGraph.Nodes.First(node => node.Name == "ThermalBarrier" && node.Kind == "PartDefinition");
        var delete = service.DeleteElement(tempRoot, createdNode.StableId);
        AssertTrue(delete.Succeeded);

        var deletedGraph = service.ParseRepository(tempRoot);
        AssertTrue(deletedGraph.Nodes.All(node => node.Name != "ThermalBarrier"));
    }
    finally
    {
        Directory.Delete(tempRoot, recursive: true);
    }
});

Run("semantic_branch_diff", () =>
{
    var diff = service.CompareBranches(Path.Combine(fixtures, "branch-divergence/base"), Path.Combine(fixtures, "branch-divergence/head"));
    AssertJsonEquals(ReadFixture("branch-divergence/expected/diff.json"), ToJson(diff));
});

Run("branch_trace_links", () =>
{
    var diff = service.CompareBranches(Path.Combine(fixtures, "branch-divergence/base"), Path.Combine(fixtures, "branch-divergence/head"));
    AssertEqual("model/root.sysml", diff.ChangedFiles.Single().Path);
    AssertEqual("ThermalMonitor", diff.Added.Single().Name);
    AssertEqual("AddsModelItem", diff.TraceLinks.Single().Relationship);
});

Run("multi_context_view_scopes_ids", () =>
{
    var view = service.BuildMultiContextView(Path.Combine(fixtures, "branch-divergence/base"), Path.Combine(fixtures, "branch-divergence/head"));
    AssertJsonEquals(ReadFixture("branch-divergence/expected/multi-context-view.json"), ToJson(view));
});

Run("git_branch_diff_and_status", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var diff = service.CompareGitBranches(repoRoot, "main", "concept-ev");
        AssertEqual("ThermalMonitor", diff.Added.Single().Name);
        AssertEqual("model/root.sysml", diff.ChangedFiles.Single().Path);

        var status = service.GetGitStatus(repoRoot);
        AssertEqual("main", status.Branch);
        AssertTrue(!status.IsDirty);
        AssertTrue(!string.IsNullOrWhiteSpace(status.HeadSha));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("git_commit_persists_changes", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        File.AppendAllText(Path.Combine(repoRoot, "model/root.sysml"), "\n  part def CoolingBar;\n");
        var statusBefore = service.GetGitStatus(repoRoot);
        AssertTrue(statusBefore.IsDirty);
        AssertEqual("model/root.sysml", statusBefore.ChangedFiles.Single().Path);

        var commit = service.CommitAll(repoRoot, "Add CoolingBar part definition");
        AssertTrue(commit.Succeeded);
        AssertTrue(!string.IsNullOrWhiteSpace(commit.CommitSha));

        var statusAfter = service.GetGitStatus(repoRoot);
        AssertTrue(!statusAfter.IsDirty);
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("merge_conflict_preview_detects_conflict", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        RunGit(repoRoot, "checkout", "-b", "left");
        File.WriteAllText(Path.Combine(repoRoot, "model/root.sysml"), GitBranchContent("LeftMonitor"));
        RunGit(repoRoot, "commit", "-am", "Left branch edit");

        RunGit(repoRoot, "checkout", "main");
        RunGit(repoRoot, "checkout", "-b", "right");
        File.WriteAllText(Path.Combine(repoRoot, "model/root.sysml"), GitBranchContent("RightMonitor"));
        RunGit(repoRoot, "commit", "-am", "Right branch edit");

        var preview = service.PreviewMergeConflict(repoRoot, "left", "right");
        AssertTrue(preview.HasConflicts);
        AssertTrue(preview.ConflictingFiles.Contains("model/root.sysml"));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("open_repository_creates_workspace_context", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        AssertEqual($"repo-{Path.GetFileName(repoRoot)}", result.RepositoryId);
        AssertTrue(result.IsWritable);
        AssertTrue(result.Graph.Nodes.Count > 0);

        var contexts = service.ListWorkspaceContexts();
        AssertTrue(contexts.Contexts.Any(c => c.RepositoryId == result.RepositoryId));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("close_workspace_context_removes_from_list", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var before = service.ListWorkspaceContexts();
        AssertTrue(before.Contexts.Any(c => c.WorkspaceId == result.WorkspaceId));

        var closed = service.CloseWorkspaceContext(result.WorkspaceId);
        AssertTrue(closed);

        var after = service.ListWorkspaceContexts();
        AssertTrue(after.Contexts.All(c => c.WorkspaceId != result.WorkspaceId));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("get_workspace_graph_returns_parsed_repo", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var graph = service.GetWorkspaceGraph(result.WorkspaceId);
        AssertTrue(graph is not null);
        AssertEqual(result.WorkspaceId, graph!.Context.WorkspaceId);
        AssertTrue(graph.Nodes.Count > 0);
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("saved_view_crud", () =>
{
    var create = service.CreateSavedView(new SavedViewCreateDto(
        "Safety Lens", "CustomWorkspaceView",
        "workspace-main", "repo-vehicle", "main",
        [], [], null, new Dictionary<string, string>
        {
            ["includeTypes"] = "Requirement,Part",
            ["layout"] = "hierarchical"
        },
        "shared"));
    AssertEqual("Safety Lens", create.Name);
    AssertEqual("shared", create.StorageMode);

    var list = service.ListSavedViews();
    AssertTrue(list.Views.Any(v => v.ViewId == create.ViewId));

    var got = service.GetSavedView(create.ViewId);
    AssertEqual("Safety Lens", got!.Name);

    var updated = service.UpdateSavedView(create.ViewId, new SavedViewUpdateDto(
        "Safety Lens v2", null, null, null, null));
    AssertEqual("Safety Lens v2", updated!.Name);

    var deleted = service.DeleteSavedView(create.ViewId);
    AssertTrue(deleted);

    var afterDelete = service.ListSavedViews();
    AssertTrue(afterDelete.Views.All(v => v.ViewId != create.ViewId));
});

Run("build_trace_matrix", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var matrix = service.BuildTraceMatrix(result.WorkspaceId);
        AssertEqual("TraceMatrix", matrix.Kind);
        AssertTrue(matrix.SourceNodeIds.Count > 0);
        AssertTrue(matrix.TargetNodeIds.Count > 0);
        AssertTrue(matrix.Cells.Count > 0);
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("open_repository_creates_workspace_context", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        AssertEqual($"repo-{Path.GetFileName(repoRoot)}", result.RepositoryId);
        AssertTrue(result.IsWritable);
        AssertTrue(result.Graph.Nodes.Count > 0);

        var contexts = service.ListWorkspaceContexts();
        AssertTrue(contexts.Contexts.Any(c => c.RepositoryId == result.RepositoryId));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("close_workspace_context_removes_from_list", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var before = service.ListWorkspaceContexts();
        AssertTrue(before.Contexts.Any(c => c.WorkspaceId == result.WorkspaceId));

        var closed = service.CloseWorkspaceContext(result.WorkspaceId);
        AssertTrue(closed);

        var after = service.ListWorkspaceContexts();
        AssertTrue(after.Contexts.All(c => c.WorkspaceId != result.WorkspaceId));
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("get_workspace_graph_returns_parsed_repo", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var graph = service.GetWorkspaceGraph(result.WorkspaceId);
        AssertTrue(graph is not null);
        AssertEqual(result.WorkspaceId, graph!.Context.WorkspaceId);
        AssertTrue(graph.Nodes.Count > 0);
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

Run("saved_view_crud", () =>
{
    var create = service.CreateSavedView(new SavedViewCreateDto(
        "Safety Lens", "CustomWorkspaceView",
        "workspace-main", "repo-vehicle", "main",
        [], [], null, new Dictionary<string, string>
        {
            ["includeTypes"] = "Requirement,Part",
            ["layout"] = "hierarchical"
        },
        "shared"));
    AssertEqual("Safety Lens", create.Name);
    AssertEqual("shared", create.StorageMode);

    var list = service.ListSavedViews();
    AssertTrue(list.Views.Any(v => v.ViewId == create.ViewId));

    var got = service.GetSavedView(create.ViewId);
    AssertEqual("Safety Lens", got!.Name);

    var updated = service.UpdateSavedView(create.ViewId, new SavedViewUpdateDto(
        "Safety Lens v2", null, null, null, null));
    AssertEqual("Safety Lens v2", updated!.Name);

    var deleted = service.DeleteSavedView(create.ViewId);
    AssertTrue(deleted);

    var afterDelete = service.ListSavedViews();
    AssertTrue(afterDelete.Views.All(v => v.ViewId != create.ViewId));
});

Run("build_trace_matrix", () =>
{
    var repoRoot = CreateTemporaryGitRepo();
    try
    {
        var result = service.OpenRepository(repoRoot);
        var matrix = service.BuildTraceMatrix(result.WorkspaceId);
        AssertEqual("TraceMatrix", matrix.Kind);
        AssertTrue(matrix.SourceNodeIds.Count > 0);
        AssertTrue(matrix.TargetNodeIds.Count > 0);
        AssertTrue(matrix.Cells.Count > 0);
    }
    finally
    {
        Directory.Delete(repoRoot, recursive: true);
    }
});

if (failures.Count > 0)
{
    Console.Error.WriteLine("Backend tests failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }
    return 1;
}

Console.WriteLine("backend-domain-tests: passed");
return 0;

void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"{name}: passed");
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: {ex.Message}");
    }
}

string ReadFixture(string path) => File.ReadAllText(Path.Combine(fixtures, path));

string CopyFixtureToTemp(string fixture)
{
    var source = Path.Combine(fixtures, fixture);
    var target = Path.Combine(Path.GetTempPath(), $"sysml2-editor-{fixture}-{Guid.NewGuid():N}");
    foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(directory.Replace(source, target, StringComparison.Ordinal));
    }

    Directory.CreateDirectory(target);
    foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
    {
        var destination = file.Replace(source, target, StringComparison.Ordinal);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(file, destination);
    }

    return target;
}

string CreateTemporaryGitRepo()
{
    var root = Path.Combine(Path.GetTempPath(), $"sysml2-editor-git-{Guid.NewGuid():N}");
    Directory.CreateDirectory(Path.Combine(root, "model"));
    File.WriteAllText(Path.Combine(root, "model/root.sysml"), GitBranchContent("BatteryPack"));

    RunGit(root, "init", "-b", "main");
    RunGit(root, "config", "user.name", "Sysml2Editor");
    RunGit(root, "config", "user.email", "sysml2editor@example.com");
    RunGit(root, "add", ".");
    RunGit(root, "commit", "-m", "Initial commit");

    RunGit(root, "checkout", "-b", "concept-ev");
    File.WriteAllText(Path.Combine(root, "model/root.sysml"), GitBranchContent("ThermalMonitor"));
    RunGit(root, "commit", "-am", "Add ThermalMonitor");
    RunGit(root, "checkout", "main");

    return root;
}

string GitBranchContent(string extraPart)
{
    var lines = new List<string>
    {
        "@Sysml2EditorIdentity { id = \"00000000-0000-4000-8000-000000000001\"; }",
        "package Vehicle {",
        "  @Sysml2EditorIdentity { id = \"55555555-5555-4555-8555-555555555555\"; }",
        "  part def BatteryPack;",
    };

    if (!string.IsNullOrWhiteSpace(extraPart))
    {
        lines.Add(string.Empty);
        lines.Add($"  @Sysml2EditorIdentity {{ id = \"{Guid.NewGuid():N}\"; }}");
        lines.Add($"  part def {extraPart};");
    }

    lines.Add("}");
    return string.Join("\n", lines) + "\n";
}

void RunGit(string repositoryRoot, params string[] arguments)
{
    var startInfo = new ProcessStartInfo("git")
    {
        WorkingDirectory = repositoryRoot,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
    };

    foreach (var argument in arguments)
    {
        startInfo.ArgumentList.Add(argument);
    }

    using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start git.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    process.WaitForExit();
    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"git {string.Join(" ", arguments)} failed: {stderr.Trim()}");
    }
}

string ToJson<T>(T value) => JsonSerializer.Serialize(value, jsonOptions);

void AssertJsonEquals(string expectedJson, string actualJson)
{
    var expected = JsonNode.Parse(expectedJson);
    var actual = JsonNode.Parse(actualJson);
    if (!JsonNode.DeepEquals(expected, actual))
    {
        throw new InvalidOperationException($"JSON mismatch.\nExpected: {expected?.ToJsonString(jsonOptions)}\nActual:   {actual?.ToJsonString(jsonOptions)}");
    }
}

void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected {expected}, got {actual}.");
    }
}

void AssertTrue(bool condition)
{
    if (!condition)
    {
        throw new InvalidOperationException("Condition was false.");
    }
}

string FindRepositoryRoot()
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
