using HtmlAgilityPack;
using Markdig.Helpers;
using Markdig.Syntax;
using System.Text;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="HtmlBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{HtmlBlock}" />
    public class HtmlBlockRenderer : XmlDocObjectRenderer<HtmlBlock>
    {
        protected override void Write(XmlDocRender renderer, HtmlBlock obj)
        {
            if (!renderer.ImplicitParagraph)
            {
                _ = renderer.Write("<para>");
            }
            if (obj.Lines.Lines is StringLine[] lines)
            {
                List<string> builder = new(lines.Length);
                foreach (StringLine line in lines)
                {
                    string lineText = line.Slice.ToString();
                    if (!string.IsNullOrWhiteSpace(lineText))
                    {
                        builder.Add(lineText);
                    }
                }

                HtmlDocument doc = new();
                doc.LoadHtml(string.Join('\n', builder));
                renderer.WriteHtml(doc.DocumentNode.ChildNodes);
            }
            if (!renderer.ImplicitParagraph)
            {
                _ = renderer.Write("</para>");
            }
        }
    }
}
