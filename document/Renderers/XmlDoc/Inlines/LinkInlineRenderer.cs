using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{LinkInline}" />
    public class LinkInlineRenderer : XmlDocObjectRenderer<LinkInline>
    {
        protected override void Write(XmlDocRender renderer, LinkInline link)
        {
            if (link.IsImage)
            {
                renderer.Write("![");
                renderer.WriteChildren(link);
                renderer.Write("](")
                    .WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url);

                if (!string.IsNullOrEmpty(link.Title))
                {
                    renderer.Write(" \"")
                        .WriteEscape(link.Title)
                        .Write('"');
                }

                renderer.Write(')');
            }
            else
            {
                renderer.Write("<see href=\"")
                    .WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url)
                    .Write('"');

                if (!string.IsNullOrEmpty(link.Title))
                {
                    renderer.Write(" title=\"")
                        .WriteEscape(link.Title)
                        .Write('"');
                }

                renderer.Write('>');
                renderer.WriteChildren(link);
                renderer.Write("</see>");
            }
        }
    }
}
