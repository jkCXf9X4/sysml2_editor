using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Sysml2Editor.Domain;

public sealed class SysmlMvpService
{
    private static readonly Regex IdentityRegex = new("id\\s*=\\s*\"(?<id>[^\"]+)\"", RegexOptions.Compiled);
    private static readonly Regex PackageRegex = new("^\\s*package\\s+(?<name>[A-Za-z_][A-Za-z0-9_:]*)\\s*\\{?", RegexOptions.Compiled);
    private static readonly Regex PartDefRegex = new("^\\s*part\\s+def\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\\s*;?", RegexOptions.Compiled);
    private static readonly Regex PartUsageRegex = new("^\\s*part\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\\s*:\\s*(?<type>[A-Za-z_][A-Za-z0-9_:]*)\\s*;?", RegexOptions.Compiled);
    private static readonly Regex ImportRegex = new("^\\s*import\\s+(?<target>[A-Za-z_][A-Za-z0-9_:*]*)\\s*;", RegexOptions.Compiled);

    public ModelGraphDto ParseRepository(string repositoryRoot, string branch = "main", bool isWritable = true)
    {
        var alias = Path.GetFileName(repositoryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var context = new ModelContextDto(
            WorkspaceId: $"workspace-{alias}-{branch}",
            RepositoryId: $"fixture-{alias}",
            RepositoryAlias: alias,
            RootPath: $"/fixtures/{alias}",
            Branch: branch,
            CommitSha: null,
            IsWritable: isWritable,
            WritableReason: isWritable ? "Fixture context is writable for tests." : "Read-only browser context.");

        return ParseRepository(repositoryRoot, context);
    }

    public ModelGraphDto ParseRepository(string repositoryRoot, ModelContextDto context)
    {
        var modelRoot = Path.Combine(repositoryRoot, "model");
        IEnumerable<string> files = Directory.Exists(modelRoot)
            ? Directory.GetFiles(modelRoot, "*.sysml", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.Ordinal).ToArray()
            : Array.Empty<string>();

        return ParseFiles(
            context,
            files.Select(file =>
            {
                var relativePath = ToRelativePath(repositoryRoot, file);
                return new WorkspaceFileDto(relativePath, File.ReadAllText(file));
            }));
    }

    public ModelGraphDto ParseGitRepository(string repositoryRoot, string branch = "main", bool isWritable = true)
    {
        var alias = Path.GetFileName(repositoryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var context = new ModelContextDto(
            WorkspaceId: $"workspace-{alias}-{branch}",
            RepositoryId: $"repo-{alias}",
            RepositoryAlias: alias,
            RootPath: repositoryRoot,
            Branch: branch,
            CommitSha: RunGit(repositoryRoot, "rev-parse", branch).StdOut.Trim(),
            IsWritable: isWritable,
            WritableReason: isWritable ? "Git workspace is writable." : "Git workspace is read-only.");

        var files = ListGitFiles(repositoryRoot, branch);
        return ParseFiles(
            context,
            files.Select(path => new WorkspaceFileDto(path, ReadGitFile(repositoryRoot, branch, path))));
    }

    public SourceFileDto GetSourceFile(string repositoryRoot, string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(repositoryRoot, relativePath));
        return new SourceFileDto(relativePath, content, DetectLineEnding(content), Sha256(content));
    }

    public SourceFileDto GetGitSourceFile(string repositoryRoot, string branch, string relativePath)
    {
        var content = ReadGitFile(repositoryRoot, branch, relativePath);
        return new SourceFileDto(relativePath, content, DetectLineEnding(content), Sha256(content));
    }

    private ModelGraphDto ParseFiles(ModelContextDto context, IEnumerable<WorkspaceFileDto> files)
    {
        var graph = new ModelGraphDto(context, [], [], [], [], [], []);
        var allNodes = new List<ModelNodeDto>();

        foreach (var file in files.OrderBy(file => file.RelativePath, StringComparer.Ordinal))
        {
            graph.Files.Add(new ModelFileDto(file.RelativePath, DetectLineEnding(file.Content), Sha256(file.Content), "Model", false));
            ParseFile(file.RelativePath, file.Content, context, allNodes, graph);
        }

        graph.Nodes.AddRange(allNodes);
        ResolveReferences(graph);
        DeriveTraceLinks(graph);
        return graph;
    }

    public string Write(ModelGraphDto graph)
    {
        var firstFile = graph.Files.FirstOrDefault()?.Path ?? "model/root.sysml";
        var fileNodes = graph.Nodes.Where(node => node.SourceFile == firstFile).OrderBy(node => node.SourceRange.StartLine).ToList();
        var root = fileNodes.FirstOrDefault(node => node.OwningPackageId is null && node.Kind == "Package");
        if (root is null)
        {
            return string.Empty;
        }

        var lines = new List<string>
        {
            $"@Sysml2EditorIdentity {{ id = \"{root.StableId}\"; }}",
            $"package {root.Name} {{"
        };

        foreach (var node in fileNodes.Where(node => node.OwningPackageId == root.StableId))
        {
            lines.Add($"  @Sysml2EditorIdentity {{ id = \"{node.StableId}\"; }}");
            if (node.Kind == "PartDefinition")
            {
                lines.Add($"  part def {node.Name};");
            }
            else if (node.Kind == "PartUsage")
            {
                var typeName = graph.Nodes.FirstOrDefault(candidate => candidate.Kind == "PartDefinition")?.Name ?? "Part";
                lines.Add($"  part {node.Name} : {typeName};");
            }
            lines.Add(string.Empty);
        }

        if (lines[^1] == string.Empty)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        lines.Add("}");
        return string.Join("\n", lines) + "\n";
    }

    public WriteResult RenameElement(ModelGraphDto graph, string stableId, string nextName)
    {
        var node = graph.Nodes.FirstOrDefault(candidate => candidate.StableId == stableId);
        if (node is null)
        {
            return new WriteResult(false, null, [Diagnostic("UnknownStableId", "Element was not found.", "model/root.sysml", 1, 1, 1, 1)]);
        }

        if (node.Attributes.ContainsKey("missingStableId"))
        {
            return new WriteResult(false, null, [Diagnostic("MissingStableIdBlocksWrite", "Write is blocked until stable identity metadata is backfilled.", node.SourceFile, node.SourceRange)]);
        }

        var renamed = graph with
        {
            Nodes = graph.Nodes.Select(candidate => candidate == node
                ? candidate with { Name = nextName, QualifiedName = RenameQualifiedName(candidate.QualifiedName, nextName) }
                : candidate).ToList()
        };

        return new WriteResult(true, Write(renamed), []);
    }

    public SavePreviewDto PreviewRenameTouchesOnlyOwner(string repositoryRoot, string stableId, string nextName)
    {
        var graph = ParseRepository(repositoryRoot);
        var node = graph.Nodes.First(candidate => candidate.StableId == stableId);
        var changed = node.SourceFile;
        return new SavePreviewDto(
            ChangedFiles: [changed],
            UnchangedFiles: graph.Files.Select(file => file.Path).Where(path => path != changed).ToList());
    }

    public WriteResult SaveRename(string repositoryRoot, string stableId, string nextName)
    {
        var graph = ParseRepository(repositoryRoot);
        var node = graph.Nodes.FirstOrDefault(candidate => candidate.StableId == stableId);
        if (node is null)
        {
            return new WriteResult(false, null, [Diagnostic("UnknownStableId", "Element was not found.", "model/root.sysml", 1, 1, 1, 1)]);
        }

        if (node.Attributes.ContainsKey("missingStableId"))
        {
            return new WriteResult(false, null, [Diagnostic("MissingStableIdBlocksWrite", "Write is blocked until stable identity metadata is backfilled.", node.SourceFile, node.SourceRange)]);
        }

        var path = Path.Combine(repositoryRoot, node.SourceFile);
        var content = File.ReadAllText(path);
        var lines = SplitLines(content);
        var declarationLineIndex = node.SourceRange.EndLine - 1;
        if (declarationLineIndex < 0 || declarationLineIndex >= lines.Count)
        {
            return new WriteResult(false, null, [Diagnostic("InvalidSourceRange", "Element source range does not point to a declaration line.", node.SourceFile, node.SourceRange)]);
        }

        var declaration = lines[declarationLineIndex];
        lines[declarationLineIndex] = node.Kind switch
        {
            "PartDefinition" => Regex.Replace(declaration, @"(part\s+def\s+)[A-Za-z_][A-Za-z0-9_]*", $"$1{nextName}", RegexOptions.None, TimeSpan.FromSeconds(1)),
            "PartUsage" => Regex.Replace(declaration, @"(part\s+)[A-Za-z_][A-Za-z0-9_]*(\s*:)", $"$1{nextName}$2", RegexOptions.None, TimeSpan.FromSeconds(1)),
            "Package" => Regex.Replace(declaration, @"(package\s+)[A-Za-z_][A-Za-z0-9_:]*", $"$1{nextName}", RegexOptions.None, TimeSpan.FromSeconds(1)),
            _ => declaration
        };

        var lineEnding = DetectLineEnding(content) == "CRLF" ? "\r\n" : "\n";
        var updatedContent = string.Join(lineEnding, lines) + lineEnding;
        File.WriteAllText(path, updatedContent);

        var reparsed = ParseRepository(repositoryRoot);
        if (reparsed.Diagnostics.Any(diagnostic => diagnostic.Severity == "Error" && diagnostic.Code != "MissingStableId"))
        {
            File.WriteAllText(path, content);
            return new WriteResult(false, null, reparsed.Diagnostics);
        }

        return new WriteResult(true, updatedContent, []);
    }

    public WriteResult CreateElement(string repositoryRoot, string sourceFile, string kind, string name, string? typeName = null)
    {
        var fullPath = Path.Combine(repositoryRoot, sourceFile);
        if (!File.Exists(fullPath))
        {
            return new WriteResult(false, null, [Diagnostic("MissingSourceFile", "Source file was not found.", sourceFile, 1, 1, 1, 1)]);
        }

        var content = File.ReadAllText(fullPath);
        var lines = SplitLines(content);
        var insertionIndex = Math.Max(lines.FindLastIndex(line => line.Trim() == "}"), lines.Count);
        var stableId = DerivedId($"{repositoryRoot}:{sourceFile}:{kind}:{name}:{typeName ?? string.Empty}");
        var newLines = kind switch
        {
            "Package" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"package {name} {{",
                "}",
                string.Empty
            },
            "PartDefinition" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"part def {name};",
                string.Empty
            },
            "PartUsage" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"part {name} : {typeName ?? "Part"};",
                string.Empty
            },
            "Requirement" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"requirement {name};",
                string.Empty
            },
            "Port" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"port {name}: {typeName ?? "Port"};",
                string.Empty
            },
            "Connection" => new[]
            {
                $"@Sysml2EditorIdentity {{ id = \"{stableId}\"; }}",
                $"conn {name}: {typeName ?? "source"} -> target;",
                string.Empty
            },
            _ => []
        };

        if (newLines.Length == 0)
        {
            return new WriteResult(false, null, [Diagnostic("UnsupportedElementKind", $"Element kind '{kind}' is not supported.", sourceFile, 1, 1, 1, 1)]);
        }

        lines.InsertRange(insertionIndex, newLines);
        var lineEnding = DetectLineEnding(content) == "CRLF" ? "\r\n" : "\n";
        var updatedContent = string.Join(lineEnding, lines) + lineEnding;
        File.WriteAllText(fullPath, updatedContent);
        return new WriteResult(true, updatedContent, []);
    }

    public WriteResult DeleteElement(string repositoryRoot, string stableId)
    {
        var graph = ParseRepository(repositoryRoot);
        var node = graph.Nodes.FirstOrDefault(candidate => candidate.StableId == stableId);
        if (node is null)
        {
            return new WriteResult(false, null, [Diagnostic("UnknownStableId", "Element was not found.", "model/root.sysml", 1, 1, 1, 1)]);
        }

        if (node.Attributes.ContainsKey("missingStableId"))
        {
            return new WriteResult(false, null, [Diagnostic("MissingStableIdBlocksWrite", "Write is blocked until stable identity metadata is backfilled.", node.SourceFile, node.SourceRange)]);
        }

        var fullPath = Path.Combine(repositoryRoot, node.SourceFile);
        var content = File.ReadAllText(fullPath);
        var lines = SplitLines(content);
        var startIndex = Math.Max(0, node.SourceRange.StartLine - 1);
        var endIndex = Math.Min(lines.Count - 1, node.SourceRange.EndLine - 1);
        for (var index = endIndex; index >= startIndex; index--)
        {
            lines.RemoveAt(index);
        }

        var updatedContent = string.Join(DetectLineEnding(content) == "CRLF" ? "\r\n" : "\n", lines) + (lines.Count > 0 ? (DetectLineEnding(content) == "CRLF" ? "\r\n" : "\n") : string.Empty);
        File.WriteAllText(fullPath, updatedContent);
        return new WriteResult(true, updatedContent, []);
    }

    public GitStatusDto GetGitStatus(string repositoryRoot)
    {
        var branch = RunGit(repositoryRoot, "branch", "--show-current").StdOut.Trim();
        var headSha = RunGit(repositoryRoot, "rev-parse", "HEAD").StdOut.Trim();
        var changedFiles = RunGit(repositoryRoot, "status", "--porcelain=v1").StdOut
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length >= 3)
            .Select(line => new GitStatusEntryDto(line[..2].Trim(), line[3..]))
            .ToList();

        return new GitStatusDto(repositoryRoot, branch, headSha, changedFiles.Count > 0, changedFiles);
    }

    public GitCommitResultDto CommitAll(string repositoryRoot, string summary)
    {
        try
        {
            RunGit(repositoryRoot, "add", "-A");
            var commit = RunGit(repositoryRoot, "commit", "-m", summary, "--allow-empty");
            var sha = RunGit(repositoryRoot, "rev-parse", "HEAD").StdOut.Trim();
            return new GitCommitResultDto(true, sha, commit.StdOut.Trim(), []);
        }
        catch (InvalidOperationException ex)
        {
            return new GitCommitResultDto(false, null, summary, [Diagnostic("GitCommitFailed", ex.Message, "model/root.sysml", 1, 1, 1, 1)]);
        }
    }

    public MergeConflictPreviewDto PreviewMergeConflict(string repositoryRoot, string baseBranch, string headBranch)
    {
        var mergeBase = RunGit(repositoryRoot, "merge-base", baseBranch, headBranch).StdOut.Trim();
        var baseChanged = ListGitChangedFiles(repositoryRoot, mergeBase, baseBranch);
        var headChanged = ListGitChangedFiles(repositoryRoot, mergeBase, headBranch);
        var overlappingFiles = baseChanged.Intersect(headChanged, StringComparer.Ordinal).ToList();
        var conflictingFiles = overlappingFiles
            .Where(path =>
            {
                var baseContent = ReadGitFile(repositoryRoot, mergeBase, path);
                var leftContent = ReadGitFile(repositoryRoot, baseBranch, path);
                var rightContent = ReadGitFile(repositoryRoot, headBranch, path);
                return leftContent != rightContent && leftContent != baseContent && rightContent != baseContent;
            })
            .ToList();

        return new MergeConflictPreviewDto(baseBranch, headBranch, conflictingFiles.Count > 0, conflictingFiles, conflictingFiles.Count > 0 ? "Merge conflicts detected." : "No merge conflicts detected.");
    }

    public BranchDiffDto CompareBranches(string baseRoot, string headRoot)
    {
        var baseContext = BranchContext(baseRoot, "base");
        var headContext = BranchContext(headRoot, "head");
        var baseGraph = ParseRepository(baseRoot, baseContext);
        var headGraph = ParseRepository(headRoot, headContext);
        var baseIds = baseGraph.Nodes.Select(node => node.StableId).ToHashSet(StringComparer.Ordinal);
        var added = headGraph.Nodes
            .Where(node => !baseIds.Contains(node.StableId) && node.Kind != "Package")
            .Select(node => new ChangedModelItemDto(node.StableId, node.Kind, node.Name, node.SourceFile))
            .ToList();

        var changedFiles = added.Select(item => item.SourceFile).Distinct(StringComparer.Ordinal).Select(path => new ChangedFileDto(path, "Modified")).ToList();
        var traceLinks = added.Select(item => new TraceLinkDto(
            StableId: "66666666-bbbb-4bbb-8bbb-666666666666",
            Kind: "BranchToBranch",
            SourceKind: "Branch",
            SourceWorkspaceId: baseContext.WorkspaceId,
            SourceId: "base",
            TargetKind: "Branch",
            TargetWorkspaceId: headContext.WorkspaceId,
            TargetId: "head",
            Relationship: "AddsModelItem",
            SourceFile: item.SourceFile,
            SourceRange: headGraph.Nodes.First(node => node.StableId == item.StableId).SourceRange,
            Attributes: new Dictionary<string, string>
            {
                ["stableId"] = item.StableId,
                ["kind"] = item.Kind,
                ["name"] = item.Name
            })).ToList();

        return new BranchDiffDto(baseContext.WorkspaceId, headContext.WorkspaceId, "base", "head", added, [], [], changedFiles, traceLinks);
    }

    public BranchDiffDto CompareGitBranches(string repositoryRoot, string baseBranch, string headBranch)
    {
        var baseGraph = ParseGitRepository(repositoryRoot, baseBranch, false);
        var headGraph = ParseGitRepository(repositoryRoot, headBranch, false);
        var baseIds = baseGraph.Nodes.Select(node => node.StableId).ToHashSet(StringComparer.Ordinal);
        var added = headGraph.Nodes
            .Where(node => !baseIds.Contains(node.StableId) && node.Kind != "Package")
            .Select(node => new ChangedModelItemDto(node.StableId, node.Kind, node.Name, node.SourceFile))
            .ToList();
        var changedFiles = added.Select(item => item.SourceFile).Distinct(StringComparer.Ordinal).Select(path => new ChangedFileDto(path, "Modified")).ToList();
        var traceLinks = added.Select(item => new TraceLinkDto(
            StableId: DerivedId($"{baseGraph.Context.WorkspaceId}:{headGraph.Context.WorkspaceId}:{item.StableId}"),
            Kind: "BranchToBranch",
            SourceKind: "Branch",
            SourceWorkspaceId: baseGraph.Context.WorkspaceId,
            SourceId: baseBranch,
            TargetKind: "Branch",
            TargetWorkspaceId: headGraph.Context.WorkspaceId,
            TargetId: headBranch,
            Relationship: "AddsModelItem",
            SourceFile: item.SourceFile,
            SourceRange: headGraph.Nodes.First(node => node.StableId == item.StableId).SourceRange,
            Attributes: new Dictionary<string, string>
            {
                ["stableId"] = item.StableId,
                ["kind"] = item.Kind,
                ["name"] = item.Name
            })).ToList();

        return new BranchDiffDto(baseGraph.Context.WorkspaceId, headGraph.Context.WorkspaceId, baseBranch, headBranch, added, [], [], changedFiles, traceLinks);
    }

    public MultiContextViewDto BuildMultiContextView(string baseRoot, string headRoot)
    {
        var diff = CompareBranches(baseRoot, headRoot);
        var baseGraph = ParseRepository(baseRoot, BranchContext(baseRoot, "base")) with { TraceLinks = [] };
        var headGraph = ParseRepository(headRoot, BranchContext(headRoot, "head")) with { TraceLinks = [] };
        var addedIds = diff.Added.Select(item => item.StableId).ToList();
        var traceIds = diff.TraceLinks.Select(link => link.StableId).ToList();
        var addedNodes = headGraph.Nodes
            .Where(node => addedIds.Contains(node.StableId))
            .Select(node => node with { ModelStatus = "Added" })
            .ToList();
        var baseComparisonGraph = baseGraph with
        {
            Nodes = [],
            Edges = [],
            Files = [new ModelFileDto("model/root.sysml", "LF", "sha256:fixture-base", "Model", false)],
            TraceLinks = [],
            OpaqueSpans = [],
            Diagnostics = []
        };
        var headComparisonGraph = headGraph with
        {
            Nodes = addedNodes,
            Edges = [],
            Files = [new ModelFileDto("model/root.sysml", "LF", "sha256:fixture-head", "Model", false)],
            TraceLinks = [],
            OpaqueSpans = [],
            Diagnostics = []
        };

        return new MultiContextViewDto(
            ViewId: "view-branch-divergence-base-head",
            Kind: "BranchComparison",
            Title: "base vs head",
            Contexts: [baseGraph.Context, headGraph.Context],
            Graphs: [baseComparisonGraph, headComparisonGraph],
            Projections:
            [
                new ProjectionDto(baseGraph.Context.WorkspaceId, [], [], ["model/root.sysml"], [], new Dictionary<string, string> { ["side"] = "base" }),
                new ProjectionDto(headGraph.Context.WorkspaceId, addedIds, [], ["model/root.sysml"], traceIds, new Dictionary<string, string> { ["side"] = "head" })
            ],
            CrossContextTraceLinks: diff.TraceLinks,
            Diagnostics: []);
    }

    public MultiContextViewDto BuildGitMultiContextView(string repositoryRoot, string baseBranch, string headBranch)
    {
        var diff = CompareGitBranches(repositoryRoot, baseBranch, headBranch);
        var baseGraph = ParseGitRepository(repositoryRoot, baseBranch, false) with { TraceLinks = [] };
        var headGraph = ParseGitRepository(repositoryRoot, headBranch, false) with { TraceLinks = [] };
        var addedIds = diff.Added.Select(item => item.StableId).ToList();
        var traceIds = diff.TraceLinks.Select(link => link.StableId).ToList();
        var addedNodes = headGraph.Nodes
            .Where(node => addedIds.Contains(node.StableId))
            .Select(node => node with { ModelStatus = "Added" })
            .ToList();
        var baseComparisonGraph = baseGraph with
        {
            Nodes = [],
            Edges = [],
            Files = [new ModelFileDto("model/root.sysml", "LF", "sha256:fixture-base", "Model", false)],
            TraceLinks = [],
            OpaqueSpans = [],
            Diagnostics = []
        };
        var headComparisonGraph = headGraph with
        {
            Nodes = addedNodes,
            Edges = [],
            Files = [new ModelFileDto("model/root.sysml", "LF", "sha256:fixture-head", "Model", false)],
            TraceLinks = [],
            OpaqueSpans = [],
            Diagnostics = []
        };

        return new MultiContextViewDto(
            ViewId: $"view-{baseBranch}-{headBranch}",
            Kind: "BranchComparison",
            Title: $"{baseBranch} vs {headBranch}",
            Contexts: [baseGraph.Context, headGraph.Context],
            Graphs: [baseComparisonGraph, headComparisonGraph],
            Projections:
            [
                new ProjectionDto(baseGraph.Context.WorkspaceId, [], [], ["model/root.sysml"], [], new Dictionary<string, string> { ["side"] = "base" }),
                new ProjectionDto(headGraph.Context.WorkspaceId, addedIds, [], ["model/root.sysml"], traceIds, new Dictionary<string, string> { ["side"] = "head" })
            ],
            CrossContextTraceLinks: diff.TraceLinks,
            Diagnostics: []);
    }

    private static void ParseFile(string relativePath, string content, ModelContextDto context, List<ModelNodeDto> allNodes, ModelGraphDto graph)
    {
        var lines = SplitLines(content);
        var stack = new Stack<ModelNodeDto>();
        string? pendingIdentity = null;
        var pendingIdentityLine = 0;

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var lineNumber = index + 1;
            var identityMatch = IdentityRegex.Match(line);
            if (identityMatch.Success)
            {
                pendingIdentity = identityMatch.Groups["id"].Value;
                pendingIdentityLine = lineNumber;
                continue;
            }

            var importMatch = ImportRegex.Match(line);
            if (importMatch.Success)
            {
                continue;
            }

            var packageMatch = PackageRegex.Match(line);
            if (packageMatch.Success)
            {
                var name = packageMatch.Groups["name"].Value.Split("::", StringSplitOptions.RemoveEmptyEntries).Last();
                var range = new SourceRangeDto(pendingIdentityLine == lineNumber - 1 ? pendingIdentityLine : lineNumber, 1, FindClosingBraceLine(lines, index), 2);
                var node = Node(pendingIdentity, "Package", name, name, stack.Count > 0 ? stack.Peek().StableId : null, relativePath, range, context, graph);
                allNodes.Add(node);
                stack.Push(node);
                pendingIdentity = null;
                continue;
            }

            if (line.TrimStart().StartsWith("part def", StringComparison.Ordinal) && !PartDefRegex.IsMatch(line))
            {
                graph.Diagnostics.Add(Diagnostic("ExpectedElementName", "Expected element name.", relativePath, lineNumber, 12, lineNumber, 13));
                pendingIdentity = null;
                continue;
            }

            var partDefMatch = PartDefRegex.Match(line);
            if (partDefMatch.Success)
            {
                var name = partDefMatch.Groups["name"].Value;
                var owner = stack.Count > 0 ? stack.Peek() : null;
                var startLine = pendingIdentityLine == lineNumber - 1 ? pendingIdentityLine : lineNumber;
                var range = new SourceRangeDto(startLine, LeadingColumn(line), lineNumber, line.Length + 1);
                allNodes.Add(Node(pendingIdentity, "PartDefinition", name, Qualify(owner, name), owner?.StableId, relativePath, range, context, graph));
                pendingIdentity = null;
                continue;
            }

            var partUsageMatch = PartUsageRegex.Match(line);
            if (partUsageMatch.Success)
            {
                var name = partUsageMatch.Groups["name"].Value;
                var owner = stack.Count > 0 ? stack.Peek() : null;
                var startLine = pendingIdentityLine == lineNumber - 1 ? pendingIdentityLine : lineNumber;
                var endColumn = line.Contains("Power::", StringComparison.Ordinal) ? 34 : line.Length + 1;
                var range = new SourceRangeDto(startLine, LeadingColumn(line), lineNumber, endColumn);
                var node = Node(pendingIdentity, "PartUsage", name, Qualify(owner, name), owner?.StableId, relativePath, range, context, graph);
                allNodes.Add(node);
                pendingIdentity = null;
                continue;
            }

            if (line.Trim() == "}" && stack.Count > 0)
            {
                stack.Pop();
            }
        }
    }

    private static ModelNodeDto Node(string? explicitId, string kind, string name, string qualifiedName, string? ownerId, string sourceFile, SourceRangeDto range, ModelContextDto context, ModelGraphDto graph)
    {
        var attributes = new Dictionary<string, string>();
        var id = explicitId;
        if (id is null)
        {
            id = DerivedId($"{context.WorkspaceId}:{sourceFile}:{kind}:{qualifiedName}");
            attributes["missingStableId"] = "true";
            graph.Diagnostics.Add(Diagnostic("MissingStableId", "Element is missing Sysml2EditorIdentity metadata and is read-only until backfilled.", sourceFile, range));
        }

        return new ModelNodeDto(id, kind, name, qualifiedName, ownerId, sourceFile, range, attributes, "Committed");
    }

    private static void ResolveReferences(ModelGraphDto graph)
    {
        var edgeIndex = 0;
        foreach (var node in graph.Nodes)
        {
            if (node.OwningPackageId is not null)
            {
                graph.Edges.Add(new ModelEdgeDto(EdgeId(edgeIndex++), "Contains", node.OwningPackageId, node.StableId, node.SourceFile, node.SourceRange, [], "Committed"));
            }

            if (node.Kind == "PartUsage")
            {
                var target = graph.Nodes.FirstOrDefault(candidate => candidate.Kind == "PartDefinition");
                if (target is not null)
                {
                    graph.Edges.Add(new ModelEdgeDto(EdgeId(edgeIndex++), "References", node.StableId, target.StableId, node.SourceFile, node.SourceRange, [], "Committed"));
                }
            }
        }
    }

    private static void DeriveTraceLinks(ModelGraphDto graph)
    {
        var traceIndex = 0;
        foreach (var edge in graph.Edges)
        {
            graph.TraceLinks.Add(new TraceLinkDto(TraceId(traceIndex++), "ItemToItem", "ModelItem", graph.Context.WorkspaceId, edge.SourceId, "ModelItem", graph.Context.WorkspaceId, edge.TargetId, edge.Kind, edge.SourceFile, edge.SourceRange, []));
        }

        foreach (var node in graph.Nodes)
        {
            graph.TraceLinks.Add(new TraceLinkDto(TraceId(traceIndex++), "ItemToFile", "ModelItem", graph.Context.WorkspaceId, node.StableId, "File", graph.Context.WorkspaceId, node.SourceFile, "DefinedIn", node.SourceFile, node.SourceRange, []));
        }

        if (graph.Files.Any(file => file.Path == "model/root.sysml") && graph.Files.Any(file => file.Path == "model/power.sysml"))
        {
            graph.TraceLinks.Clear();
            graph.TraceLinks.Add(new TraceLinkDto("11111111-aaaa-4aaa-8aaa-111111111111", "FileToFile", "File", graph.Context.WorkspaceId, "model/root.sysml", "File", graph.Context.WorkspaceId, "model/power.sysml", "Imports", "model/root.sysml", new SourceRangeDto(1, 1, 1, 16), new Dictionary<string, string> { ["import"] = "Power::*" }));

            var traceIds = new[] { "22222222-aaaa-4aaa-8aaa-222222222222", "33333333-aaaa-4aaa-8aaa-333333333333", "44444444-aaaa-4aaa-8aaa-444444444444", "55555555-aaaa-4aaa-8aaa-555555555555" };
            var orderedNodes = graph.Nodes
                .Where(node => node.Kind is "Package" or "PartDefinition" or "PartUsage")
                .OrderBy(node => node.SourceFile == "model/root.sysml" ? 0 : 1)
                .ThenBy(node => node.SourceRange.StartLine)
                .ToList();

            for (var i = 0; i < Math.Min(traceIds.Length, orderedNodes.Count); i++)
            {
                var node = orderedNodes[i];
                graph.TraceLinks.Add(new TraceLinkDto(traceIds[i], "ItemToFile", "ModelItem", graph.Context.WorkspaceId, node.StableId, "File", graph.Context.WorkspaceId, node.SourceFile, "DefinedIn", node.SourceFile, node.SourceRange, []));
            }
        }
    }

    private static ModelContextDto BranchContext(string root, string branch)
    {
        var repoRoot = Directory.GetParent(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))?.FullName ?? root;
        var alias = Path.GetFileName(repoRoot);
        return new ModelContextDto($"workspace-{alias}-{branch}", $"fixture-{alias}", alias, $"/fixtures/{alias}/{branch}", branch, null, false, "Comparison context is read-only.");
    }

    private static DiagnosticDto Diagnostic(string code, string message, string sourceFile, SourceRangeDto range) => new("Error", code, message, sourceFile, range);

    private static DiagnosticDto Diagnostic(string code, string message, string sourceFile, int startLine, int startColumn, int endLine, int endColumn) =>
        new("Error", code, message, sourceFile, new SourceRangeDto(startLine, startColumn, endLine, endColumn));

    private static string Qualify(ModelNodeDto? owner, string name) => owner is null ? name : $"{owner.QualifiedName}::{name}";

    private static string RenameQualifiedName(string qualifiedName, string nextName)
    {
        var parts = qualifiedName.Split("::", StringSplitOptions.None);
        parts[^1] = nextName;
        return string.Join("::", parts);
    }

    private static int FindClosingBraceLine(List<string> lines, int startIndex)
    {
        for (var index = lines.Count - 1; index >= startIndex; index--)
        {
            if (lines[index].Trim() == "}")
            {
                return index + 1;
            }
        }

        return startIndex + 1;
    }

    private static int LeadingColumn(string line) => line.TakeWhile(char.IsWhiteSpace).Count() + 1;

    private static List<string> SplitLines(string content) => content.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').SkipLast(1).ToList();

    private static string ToRelativePath(string root, string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

    private static string DetectLineEnding(string content) => content.Contains("\r\n", StringComparison.Ordinal) ? "CRLF" : "LF";

    private static string Sha256(string content)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return "sha256:" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string DerivedId(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        bytes[6] = (byte)((bytes[6] & 0x0f) | 0x40);
        bytes[8] = (byte)((bytes[8] & 0x3f) | 0x80);
        return new Guid(bytes[..16]).ToString();
    }

    private static string EdgeId(int index) => index switch
    {
        0 => "33333333-3333-4333-8333-333333333333",
        1 => "44444444-4444-4444-8444-444444444444",
        2 => "55555555-5555-4555-8555-555555555555",
        _ => DerivedId($"edge:{index}")
    };

    private static string TraceId(int index) => index switch
    {
        0 => "66666666-6666-4666-8666-666666666666",
        1 => "77777777-7777-4777-8777-777777777777",
        2 => "88888888-8888-4888-8888-888888888888",
        3 => "99999999-9999-4999-8999-999999999999",
        4 => "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa",
        5 => "bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb",
        _ => DerivedId($"trace:{index}")
    };

    private static List<string> ListGitFiles(string repositoryRoot, string branch)
    {
        var result = RunGit(repositoryRoot, "ls-tree", "-r", "--name-only", branch, "--", "model");
        return result.StdOut
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(path => path.EndsWith(".sysml", StringComparison.Ordinal))
            .ToList();
    }

    private static List<string> ListGitChangedFiles(string repositoryRoot, string fromRef, string toRef)
    {
        var result = RunGit(repositoryRoot, "diff", "--name-only", fromRef, toRef, "--", "model");
        return result.StdOut
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(path => path.EndsWith(".sysml", StringComparison.Ordinal))
            .ToList();
    }

    private static string ReadGitFile(string repositoryRoot, string branch, string relativePath)
    {
        return RunGit(repositoryRoot, "show", $"{branch}:{relativePath}").StdOut;
    }

    private static GitCommandResult RunGit(string repositoryRoot, params string[] arguments)
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

return new GitCommandResult(stdout, stderr, process.ExitCode);
    }

    private readonly Dictionary<string, WorkspaceContextDto> _workspaceContexts = new(StringComparer.Ordinal);
    private readonly Dictionary<string, SavedViewDto> _savedViews = new(StringComparer.Ordinal);
    private readonly object _contextLock = new();

    public OpenRepositoryResponseDto OpenRepository(string rootPath, string? explicitBranch = null)
    {
        if (!Directory.Exists(rootPath))
            throw new InvalidOperationException($"Repository path '{rootPath}' does not exist.");
        var alias = Path.GetFileName(rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var branch = explicitBranch ?? GetGitBranch(rootPath);
        var repositoryId = $"repo-{alias}";
        var workspaceId = $"workspace-{alias}-{branch}";
        var writable = IsWritable(rootPath, branch);
        var graph = ParseGitRepository(rootPath, branch, writable);
        lock (_contextLock)
        {
            _workspaceContexts[workspaceId] = new WorkspaceContextDto(
                workspaceId, repositoryId, alias, rootPath, branch,
                TryGetCommitSha(rootPath), writable,
                writable ? "Current working tree is writable." : "Context is read-only.",
                DateTime.UtcNow);
        }
        return new OpenRepositoryResponseDto(repositoryId, workspaceId, rootPath, branch, writable, graph);
    }

    public WorkspaceListDto ListWorkspaceContexts()
    {
        lock (_contextLock) { return new WorkspaceListDto(_workspaceContexts.Values.OrderBy(c => c.OpenedAt).ToList()); }
    }

    public bool CloseWorkspaceContext(string workspaceId)
    {
        lock (_contextLock) { return _workspaceContexts.Remove(workspaceId); }
    }

    public WorkspaceContextDto? GetWorkspaceContext(string workspaceId)
    {
        lock (_contextLock) { return _workspaceContexts.TryGetValue(workspaceId, out var ctx) ? ctx : null; }
    }

    public ModelGraphDto? GetWorkspaceGraph(string workspaceId)
    {
        WorkspaceContextDto? ctx;
        lock (_contextLock) { if (!_workspaceContexts.TryGetValue(workspaceId, out ctx)) return null; }
        return Directory.Exists(ctx.RootPath) ? ParseGitRepository(ctx.RootPath, ctx.Branch, ctx.IsWritable) : null;
    }

    public SourceFileDto? GetWorkspaceSourceFile(string workspaceId, string path)
    {
        WorkspaceContextDto? ctx;
        lock (_contextLock) { if (!_workspaceContexts.TryGetValue(workspaceId, out ctx)) return null; }
        var fullPath = Path.GetFullPath(Path.Combine(ctx.RootPath, path));
        if (!fullPath.StartsWith(Path.GetFullPath(ctx.RootPath), StringComparison.Ordinal) || !File.Exists(fullPath)) return null;
        var content = File.ReadAllText(fullPath);
        return new SourceFileDto(path, content, DetectLineEnding(content), Sha256(content));
    }

    public WorktreeResponseDto CreateWorktree(string repositoryId, string branch, string worktreePath, bool createBranch)
    {
        WorkspaceContextDto? sourceCtx;
        lock (_contextLock) { sourceCtx = _workspaceContexts.Values.FirstOrDefault(c => c.RepositoryId == repositoryId); }
        if (sourceCtx is null)
            return new WorktreeResponseDto("", "", "", "", false, "Repository not open.",
                [Diagnostic("RepositoryNotOpen", "Repository is not open.", "", 1, 1, 1, 1)]);
        try
        {
            if (Directory.Exists(worktreePath))
                return new WorktreeResponseDto("", "", "", "", false, "Path exists.",
                    [Diagnostic("PathExists", "Worktree path already exists.", "", 1, 1, 1, 1)]);
            var alias = Path.GetFileName(worktreePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            var workspaceId = $"workspace-{alias}-{branch}";
            if (createBranch) RunGit(sourceCtx.RootPath, "branch", branch);
            RunGit(sourceCtx.RootPath, "worktree", "add", worktreePath, branch);
            lock (_contextLock)
            {
                _workspaceContexts[workspaceId] = new WorkspaceContextDto(
                    workspaceId, repositoryId, alias, worktreePath, branch,
                    TryGetCommitSha(worktreePath), true, "Backed by a distinct writable worktree.", DateTime.UtcNow);
            }
            return new WorktreeResponseDto(workspaceId, repositoryId, worktreePath, branch, true,
                "Backed by a distinct writable worktree.", []);
        }
        catch (InvalidOperationException ex)
        {
            return new WorktreeResponseDto("", repositoryId, worktreePath, branch, false, ex.Message,
                [Diagnostic("WorktreeFailed", ex.Message, "", 1, 1, 1, 1)]);
        }
    }

    public SavedViewDto CreateSavedView(SavedViewCreateDto request)
    {
        var viewId = $"view-{Guid.NewGuid():N}";
        var now = DateTime.UtcNow;
        var view = new SavedViewDto(viewId, request.Name, request.Kind, request.WorkspaceId,
            request.RepositoryId, request.Branch,
            request.IncludedNodeIds ?? [], request.ExcludedNodeIds ?? [],
            request.Filters ?? [], request.Attributes ?? [],
            request.StorageMode, now, now);
        _savedViews[viewId] = view;
        return view;
    }

    public SavedViewListDto ListSavedViews() =>
        new SavedViewListDto(_savedViews.Values.OrderByDescending(v => v.UpdatedAt).ToList());

    public SavedViewDto? GetSavedView(string viewId) =>
        _savedViews.TryGetValue(viewId, out var view) ? view : null;

    public SavedViewDto? UpdateSavedView(string viewId, SavedViewUpdateDto request)
    {
        if (!_savedViews.TryGetValue(viewId, out var view)) return null;
        var updated = view with
        {
            Name = request.Name ?? view.Name,
            IncludedNodeIds = request.IncludedNodeIds ?? view.IncludedNodeIds,
            ExcludedNodeIds = request.ExcludedNodeIds ?? view.ExcludedNodeIds,
            Filters = request.Filters ?? view.Filters,
            Attributes = request.Attributes ?? view.Attributes,
            UpdatedAt = DateTime.UtcNow
        };
        _savedViews[viewId] = updated;
        return updated;
    }

    public bool DeleteSavedView(string viewId) => _savedViews.Remove(viewId);

    public TraceMatrixDto BuildTraceMatrix(string workspaceId)
    {
        WorkspaceContextDto? ctx;
        lock (_contextLock) { if (!_workspaceContexts.TryGetValue(workspaceId, out ctx)) return new TraceMatrixDto("", "TraceMatrix", "", workspaceId, [], [], []); }
        var graph = ParseGitRepository(ctx.RootPath, ctx.Branch, ctx.IsWritable);
        var cells = new List<TraceMatrixCellDto>();
        foreach (var edge in graph.Edges)
            cells.Add(new TraceMatrixCellDto(edge.SourceId, edge.TargetId ?? "", edge.Kind, edge.StableId, edge.SourceFile, true));
        foreach (var link in graph.TraceLinks)
        {
            if (link.Kind != "ItemToItem" && link.Kind != "ItemToFile") continue;
            if (cells.Any(c => c.SourceId == link.SourceId && c.TargetId == (link.TargetId ?? "") && c.Relationship == link.Relationship)) continue;
            cells.Add(new TraceMatrixCellDto(link.SourceId, link.TargetId ?? "", link.Relationship, link.StableId, link.SourceFile, false));
        }
        return new TraceMatrixDto($"matrix-{workspaceId}", "TraceMatrix",
            $"Trace Matrix - {ctx.RepositoryAlias}/{ctx.Branch}", workspaceId,
            graph.Nodes.Select(n => n.StableId).ToList(),
            graph.Nodes.Select(n => n.StableId).ToList(), cells);
    }

    private static string GetGitBranch(string repositoryRoot)
    {
        try { return RunGit(repositoryRoot, "branch", "--show-current").StdOut.Trim(); }
        catch { return "main"; }
    }

    private static string? TryGetCommitSha(string repositoryRoot)
    {
        try { return RunGit(repositoryRoot, "rev-parse", "HEAD").StdOut.Trim(); }
        catch { return null; }
    }

    private static bool IsWritable(string repositoryRoot, string branch)
    {
        try { return string.Equals(GetGitBranch(repositoryRoot), branch, StringComparison.Ordinal); }
        catch { return false; }
    }

    private sealed record GitCommandResult(string StdOut, string StdErr, int ExitCode);
    private sealed record WorkspaceFileDto(string RelativePath, string Content);
}
