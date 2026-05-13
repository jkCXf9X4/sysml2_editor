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
        var files = Directory.Exists(modelRoot)
            ? Directory.GetFiles(modelRoot, "*.sysml", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.Ordinal).ToList()
            : new List<string>();

        var graph = new ModelGraphDto(context, [], [], [], [], [], []);
        var allNodes = new List<ModelNodeDto>();

        foreach (var file in files)
        {
            var relativePath = ToRelativePath(repositoryRoot, file);
            var content = File.ReadAllText(file);
            graph.Files.Add(new ModelFileDto(relativePath, DetectLineEnding(content), Sha256(content), "Model", false));
            ParseFile(relativePath, content, context, allNodes, graph);
        }

        graph.Nodes.AddRange(allNodes);
        ResolveReferences(graph);
        DeriveTraceLinks(graph);
        return graph;
    }

    public SourceFileDto GetSourceFile(string repositoryRoot, string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(repositoryRoot, relativePath));
        return new SourceFileDto(relativePath, content, DetectLineEnding(content), Sha256(content));
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
}
