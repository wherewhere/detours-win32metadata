using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="LiteralInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{LiteralInline}" />
    public class LiteralInlineRenderer : XmlDocObjectRenderer<LiteralInline>
    {
        protected override void Write(XmlDocRender renderer, LiteralInline obj) =>
            renderer.WriteEscape(obj.Content);
    }
}
