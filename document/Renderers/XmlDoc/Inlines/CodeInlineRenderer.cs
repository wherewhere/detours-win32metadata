using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{CodeInline}" />
    public class CodeInlineRenderer : XmlDocObjectRenderer<CodeInline>
    {
        protected override void Write(XmlDocRender renderer, CodeInline obj) =>
            _ = renderer.Write("<c>")
                .WriteEscape(obj.Content)
                .Write("</c>");
    }
}
