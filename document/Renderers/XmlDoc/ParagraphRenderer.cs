using Markdig.Syntax;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="ParagraphBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{ParagraphBlock}" />
    public class ParagraphRenderer : XmlDocObjectRenderer<ParagraphBlock>
    {
        protected override void Write(XmlDocRender renderer, ParagraphBlock obj)
        {
            if (!renderer.ImplicitParagraph)
            {
                _ = renderer.Write("<para>");
            }
            _ = renderer.WriteLeafInline(obj);
            if (!renderer.ImplicitParagraph)
            {
                _ = renderer.Write("</para>");
            }
        }
    }
}
