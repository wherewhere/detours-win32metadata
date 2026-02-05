using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for an <see cref="AutolinkInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{AutolinkInline}" />
    public class AutolinkInlineRenderer  : XmlDocObjectRenderer<AutolinkInline>
    {
        protected override void Write(XmlDocRender renderer, AutolinkInline obj)
        {
            _ = renderer.Write("<see href=\"");
            if (obj.IsEmail)
            {
                _ = renderer.Write("mailto:");
            }
            renderer.WriteEscapeUrl(obj.Url);
            renderer.Write("\">");

            renderer.WriteEscape(obj.Url);

            renderer.Write("</see>");
        }
    }
}
