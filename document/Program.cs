//#define Test

using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MessagePack;
using Microsoft.Windows.SDK.Win32Docs;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

using CancellationTokenSource cts = new();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};

#if Test
string contentBasePath = "../detours-wiki/";
string defPath = "../detours-dll/detours.def";
#else
if (args.Length < 4)
{
    Console.Error.WriteLine("USAGE: {0} <path-to-docs> <path-to-output-pack> <path-to-rsp> <path-to-def>");
    return 1;
}

string contentBasePath = args[0];
string outputPath = args[1];
string documentationMappingsRsp = args[2];
string defPath = args[3];
#endif

try
{
#if Test
    Environment.CurrentDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.ToString();
#endif

    Console.WriteLine("Parsing documents...");

    ConcurrentDictionary<string, ApiDetails> results = new();
    Dictionary<string, string> lists = Directory.EnumerateFiles(contentBasePath, "*.md").ToDictionary((Func<string, string>)Path.GetFileNameWithoutExtension!);

    bool isExports = false;
    const string baseUrl = "https://github.com/microsoft/Detours/wiki/";
    await foreach (string line in File.ReadLinesAsync(defPath, cts.Token).ConfigureAwait(false))
    {
        if (string.IsNullOrWhiteSpace(line)) { continue; }
        else if (char.IsWhiteSpace(line[0]))
        {
            if (!isExports) { continue; }
            string exportName = line.Trim();
            if (lists.TryGetValue(exportName, out string? docPath))
            {
                string markdown = await File.ReadAllTextAsync(docPath, cts.Token).ConfigureAwait(false);
                MarkdownDocument document = Markdown.Parse(markdown);
                ApiDetails details = new() { HelpLink = new Uri($"{baseUrl}{exportName}") };
                results[exportName] = details;
                ApiKind kind = ApiKind.Unknown;
                string paramName = string.Empty;
                StringBuilder builder = new();
                foreach (Block obj in document)
                {
                    switch (obj)
                    {
                        case HeadingBlock { Level: 1 }:
                            kind = ApiKind.Description;
                            break;
                        case HeadingBlock { Level: 2 } headingBlock:
                            ApplyContent(details, kind, builder, paramName);
                            SourceSpan span = headingBlock.Span;
                            string text = markdown.Substring(span.Start, span.Length).TrimStart('#').TrimEnd('\r', '\n', '-').Trim();
                            kind = text switch
                            {
                                "Remarks" => ApiKind.Remarks,
                                "Parameters" => ApiKind.Parameters,
                                "Return value" => ApiKind.ReturnValue,
                                _ => ApiKind.Unknown
                            };
                            break;
                        case ParagraphBlock paragraphBlock:
                            switch (kind)
                            {
                                case ApiKind.Description:
                                case ApiKind.Remarks:
                                case ApiKind.ReturnValue:
                                    _ = builder.Append($"<para>{Render(paragraphBlock.Inline?.FirstChild)}</para>");
                                    break;
                                case ApiKind.Parameters:
                                    if (paragraphBlock.Inline is { FirstChild: EmphasisInline { FirstChild: LiteralInline { Content: { Length: > 0 } content } } emphasisInline })
                                    {
                                        if (!string.IsNullOrEmpty(paramName))
                                        { details.Parameters[paramName] = builder.ToString(); }
                                        builder.Clear();
                                        paramName = content.ToString();
                                        _ = builder.Append($"<para>{Render(emphasisInline.NextSibling).TrimStart(' ', ':')}</para>");
                                        break;
                                    }
                                    else
                                    {
                                        goto case ApiKind.Remarks;
                                    }
                            }
                            break;
                    }
                }

                ApplyContent(details, kind, builder, paramName);

                static void ApplyContent(ApiDetails details, ApiKind kind, StringBuilder builder, string paramName = "")
                {
                    switch (kind)
                    {
                        case ApiKind.Description:
                            details.Description = builder.ToString();
                            break;
                        case ApiKind.Remarks:
                            details.Remarks = builder.ToString();
                            break;
                        case ApiKind.Parameters when !string.IsNullOrEmpty(paramName):
                            details.Parameters[paramName] = builder.ToString();
                            break;
                        case ApiKind.Fields when !string.IsNullOrEmpty(paramName):
                            details.Fields[paramName] = builder.ToString();
                            break;
                        case ApiKind.ReturnValue:
                            details.ReturnValue = builder.ToString();
                            break;
                    }
                    builder.Clear();
                }

                static string Render(Inline? first)
                {
                    if (first == null) { return string.Empty; }
                    StringBuilder builder = new();
                    do
                    {
                        switch (first)
                        {
                            case LiteralInline literalInline:
                                _ = builder.Append(WebUtility.HtmlEncode(literalInline.Content.ToString()));
                                break;
                            case AutolinkInline autolinkInline:
                                string url = WebUtility.HtmlEncode(AppendUrl(autolinkInline.Url));
                                _ = builder.Append($"<see href=\"{url}\">{url}</see>");
                                break;
                            case CodeInline codeInline:
                                _ = builder.Append($"<c>{WebUtility.HtmlEncode(codeInline.Content)}</c>");
                                break;
                            case EmphasisInline emphasisInline:
                                int count = emphasisInline.DelimiterCount % 4;
                                string inline = Render(emphasisInline.FirstChild);
                                _ = count switch
                                {
                                    1 => builder.Append($"<i>{inline}</i>"),
                                    2 => builder.Append($"<b>{inline}</b>"),
                                    3 => builder.Append($"<b><i>{inline}</i></b>"),
                                    0 or _ => builder.Append(inline),
                                };
                                break;
                            case LineBreakInline lineBreakInline:
                                _ = builder.Append(lineBreakInline.IsHard ? "<br/>" : " ");
                                break;
                            case LinkInline { IsImage: false } linkInline:
                                _ = builder.Append($"<see href=\"{WebUtility.HtmlEncode(AppendUrl(linkInline.Url))}\">{Render(linkInline.FirstChild)}</see>");
                                break;
                        }
                        [return: NotNullIfNotNull(nameof(url))]
                        static string? AppendUrl(string? url) =>
                            string.IsNullOrWhiteSpace(url) ? url : url.Contains(":/") ? url : $"{baseUrl}{url}";
                    }
                    while ((first = first.NextSibling) != null);
                    return builder.ToString();
                }
            }
        }
        else
        {
            isExports = line.Trim() == "EXPORTS";
        }
    }

#if !Test
    Console.WriteLine($"Writing results to \"{outputPath}\" and \"{documentationMappingsRsp}\".");

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    await using FileStream outputFileStream = File.OpenWrite(outputPath);
    await MessagePackSerializer.SerializeAsync(outputFileStream, results, MessagePackSerializerOptions.Standard, cts.Token).ConfigureAwait(false);

    List<string> documentationMappingsBuilder = new(results.Count + 1)
    {
        "--memberRemap"
    };

    foreach (KeyValuePair<string, ApiDetails> api in results)
    {
        documentationMappingsBuilder.Add($"{api.Key.Replace(".", "::")}=[Documentation(\"{api.Value.HelpLink}\")]");
    }

    Directory.CreateDirectory(Path.GetDirectoryName(documentationMappingsRsp)!);
    await File.WriteAllLinesAsync(documentationMappingsRsp, documentationMappingsBuilder, cts.Token).ConfigureAwait(false);
#endif
}
catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
{
    return 2;
}

return 0;

file enum ApiKind
{
    Unknown,
    Description = 1,
    Remarks,
    Parameters,
    Fields,
    ReturnValue
}