using Markdig.Syntax;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc
{
    /// <summary>
    /// An XmlDoc renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{CodeBlock}" />
    public class CodeBlockRenderer : XmlDocObjectRenderer<CodeBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockRenderer"/> class.
        /// </summary>
        public CodeBlockRenderer() { }

        protected override void Write(XmlDocRender renderer, CodeBlock obj) =>
            _ = renderer.Write("<code>").WriteLeafRawLines(obj).Write("</code>");
    }
}
