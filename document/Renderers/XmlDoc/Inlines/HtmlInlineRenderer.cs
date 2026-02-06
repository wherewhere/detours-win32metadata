using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="HtmlInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{HtmlInline}" />
    public partial class HtmlInlineRenderer : XmlDocObjectRenderer<HtmlInline>
    {
        protected override void Write(XmlDocRender renderer, HtmlInline obj)
        {
            string html = obj.Tag;
            if (TagRegex.Match(html) is { Success: true, Groups: [_, { Value: { Length: > 0 } tag }] })
            {
                string? name = tag.ToLower() switch
                {
                    "a" => "a",
                    "b" => "b",
                    "code" or "samp" or "kbd" => "c",
                    "em" => "em",
                    "i" or "cite" or "dfn" or "var" or "address" => "i",
                    "li" => "item",
                    "p" => "para",
                    "strong" => "strong",
                    "tt" => "tt",
                    "u" or "ins" => "u",
                    _ => tag,
                };
                switch (html)
                {
                    case ['<', '/', ..]:
                        _ = renderer.Write($"</{name}>");
                        break;
                    case ['<', .., '/', '>']:
                        _ = renderer.Write($"<{name}");
                        RenderAttributes(renderer, html);
                        _ = renderer.Write("/>");
                        break;
                    case ['<', .., '>']:
                        _ = renderer.Write($"<{name}");
                        RenderAttributes(renderer, html);
                        _ = renderer.Write('>');
                        break;
                }
                static void RenderAttributes(XmlDocRender renderer, string html)
                {
                    HtmlDocument doc = new();
                    doc.LoadHtml(html);
                    if (doc.DocumentNode.FirstChild is HtmlNode node)
                    {
                        foreach (HtmlAttribute attr in node.Attributes)
                        {
                            _ = renderer.Write($" {attr.Name}=\"")
                                .WriteEscape(attr.Value)
                                .Write('"');
                        }
                    }
                }
            }
        }

        [GeneratedRegex("</?(\\w+)")]
        private static partial Regex TagRegex { get; }
    }
}
