using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{LinkInline}" />
    public class LinkInlineRenderer : XmlDocObjectRenderer<LinkInline>
    {
        protected override void Write(XmlDocRender renderer, LinkInline link)
        {
            if (link.IsImage)
            {
                _ = renderer.Write("![");
                renderer.WriteChildren(link);
                _ = renderer.Write("](")
                    .WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url);

                if (!string.IsNullOrEmpty(link.Title))
                {
                    _ = renderer.Write(" \"")
                        .WriteEscape(link.Title)
                        .Write('"');
                }

                _ = renderer.Write(')');
            }
            else
            {
                _ = renderer.Write("<see href=\"")
                    .WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url)
                    .Write('"');

                if (!string.IsNullOrEmpty(link.Title))
                {
                    _ = renderer.Write(" title=\"")
                        .WriteEscape(link.Title)
                        .Write('"');
                }

                _ = renderer.Write('>');
                renderer.WriteChildren(link);
                _ = renderer.Write("</see>");
            }
        }
    }
}
