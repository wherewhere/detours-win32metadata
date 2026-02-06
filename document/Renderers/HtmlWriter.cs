using HtmlAgilityPack;

namespace Detours.Win32Metadata.Document.Renderers
{
    public static class HtmlWriter
    {
        public static void WriteHtml(this XmlDocRender renderer, HtmlNodeCollection nodes)
        {
            if (nodes == null || nodes.Count == 0) return;
            foreach (HtmlNode node in nodes)
            {
                switch (node)
                {
                    case { NodeType: HtmlNodeType.Text }:
                        _ = renderer.WriteEscape(node.InnerText);
                        break;
                    case { NodeType: HtmlNodeType.Element }:
                        switch (node.Name.ToLower())
                        {
                            case "ol":
                                _ = renderer.Write("<list type=\"number\"");
                                foreach (HtmlAttribute attr in node.Attributes)
                                {
                                    _ = renderer.Write($" {attr.Name}=\"")
                                        .WriteEscape(attr.Value)
                                        .Write('"');
                                }
                                _ = renderer.Write('>');
                                renderer.WriteHtml(node.ChildNodes);
                                _ = renderer.Write($"</list>");
                                break;
                            case "p":
                                if (!renderer.ImplicitParagraph)
                                {
                                    _ = renderer.Write("<para>");
                                }
                                renderer.WriteHtml(node.ChildNodes);
                                if (!renderer.ImplicitParagraph)
                                {
                                    _ = renderer.Write("</para>");
                                }
                                break;
                            case "pre":
                                _ = renderer.Write("<code>")
                                    .Write(string.Join("<br/>", node.InnerText.Trim('\r', '\n').Split('\n')))
                                    .Write("</code>");
                                break;
                            case "ul":
                                _ = renderer.Write("<list type=\"bullet\"");
                                foreach (HtmlAttribute attr in node.Attributes)
                                {
                                    _ = renderer.Write($" {attr.Name}=\"")
                                        .WriteEscape(attr.Value)
                                        .Write('"');
                                }
                                _ = renderer.Write('>');
                                renderer.WriteHtml(node.ChildNodes);
                                _ = renderer.Write($"</list>");
                                break;
                            case "address" or "article" or "aside" or "details" or "blockquote" or "canvas" or "dd" or "div" or "dl" or "dt" or "fieldset" or "figcaption" or "figure" or "footer" or "form" or "h1" or "h2" or "h3" or "h4" or "h5" or "h6" or "header" or "hr" or "main" or "nav" or "noscript" or "section" or "table" or "tfoot":
                                if (!renderer.ImplicitParagraph)
                                {
                                    _ = renderer.Write("<para>");
                                }
                                renderer.WriteHtml(node);
                                if (!renderer.ImplicitParagraph)
                                {
                                    _ = renderer.Write("</para>");
                                }
                                break;
                            default:
                                renderer.WriteHtml(node);
                                break;
                        }
                        break;
                }
            }
        }

        public static void WriteHtml(this XmlDocRender renderer, HtmlNode node)
        {
            string name = node.Name.ToLower() switch
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
                _ => node.Name,
            };
            _ = renderer.Write($"<{name}");
            foreach (HtmlAttribute attr in node.Attributes)
            {
                _ = renderer.Write($" {attr.Name}=\"")
                    .WriteEscape(attr.Value)
                    .Write('"');
            }
            if (node.ChildNodes.Count > 0)
            {
                _ = renderer.Write('>');
                renderer.WriteHtml(node.ChildNodes);
                _ = renderer.Write($"</{name}>");
            }
            else
            {
                _ = renderer.Write("/>");
            }
        }
    }
}
