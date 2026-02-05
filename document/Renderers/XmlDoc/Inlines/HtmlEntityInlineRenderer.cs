using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="HtmlEntityInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{HtmlEntityInline}" />
    public class HtmlEntityInlineRenderer : XmlDocObjectRenderer<HtmlEntityInline>
    {
        protected override void Write(XmlDocRender renderer, HtmlEntityInline obj) =>
            renderer.WriteEscape(obj.Transcoded);
    }
}
