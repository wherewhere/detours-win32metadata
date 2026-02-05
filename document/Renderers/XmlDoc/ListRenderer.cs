using Markdig.Syntax;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc
{
    /// <summary>
    /// A XmlDoc renderer for a <see cref="ListBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{ListBlock}" />
    public class ListRenderer : XmlDocObjectRenderer<ListBlock>
    {
        protected override void Write(XmlDocRender renderer, ListBlock listBlock)
        {
            _ = renderer.Write($"<list type=\"{(listBlock.IsOrdered ? "number" : "bullet")}\">");

            foreach (ListItemBlock listItem in listBlock.OfType<ListItemBlock>())
            {
                bool previousImplicit = renderer.ImplicitParagraph;
                renderer.ImplicitParagraph = !listBlock.IsLoose;

                _ = renderer.Write("<item>");
                renderer.WriteChildren(listItem);
                _ = renderer.Write("</item>");

                renderer.ImplicitParagraph = previousImplicit;
            }

            _ = renderer.Write("</list>");
        }
    }
}
