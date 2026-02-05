using Markdig.Renderers;
using Markdig.Syntax;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc
{
    /// <summary>
    /// A base class for XmlDoc rendering <see cref="Block"/> and <see cref="Markdig.Syntax.Inlines.Inline"/> Markdown objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="IMarkdownObjectRenderer" />
    public abstract class XmlDocObjectRenderer<TObject> : MarkdownObjectRenderer<XmlDocRender, TObject> where TObject : MarkdownObject;
}
