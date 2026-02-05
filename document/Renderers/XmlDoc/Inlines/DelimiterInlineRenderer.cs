using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="DelimiterInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{DelimiterInline}" />
    public class DelimiterInlineRenderer : XmlDocObjectRenderer<DelimiterInline>
    {
        protected override void Write(XmlDocRender renderer, DelimiterInline obj) =>
            renderer.WriteEscape(obj.ToLiteral()).WriteChildren(obj);
    }
}
