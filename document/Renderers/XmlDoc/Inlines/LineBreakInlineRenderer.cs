using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{LineBreakInline}" />
    public class LineBreakInlineRenderer : XmlDocObjectRenderer<LineBreakInline>
    {
        protected override void Write(XmlDocRender renderer, LineBreakInline obj)
        {
            if (renderer.IsLastInContainer) { return; }
            _ = obj.IsHard ? renderer.Write("<br/>") : renderer.Write(" ");
        }
    }
}
