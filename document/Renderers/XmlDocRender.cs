using Detours.Win32Metadata.Document.Renderers.XmlDoc;
using Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using System.Net;
using System.Runtime.CompilerServices;

namespace Detours.Win32Metadata.Document.Renderers
{
    public class XmlDocRender : TextRendererBase<XmlDocRender>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use implicit paragraph (optional &lt;p&gt;)
        /// </summary>
        public bool ImplicitParagraph { get; set; }

        /// <summary>
        /// Gets a value to use as the base url for all relative links
        /// </summary>
        public Uri? BaseUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlRenderer"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public XmlDocRender(TextWriter writer) : base(writer)
        {
            // Default block renderers
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new HtmlBlockRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());

            // Default inline renderers
            ObjectRenderers.Add(new AutolinkInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new DelimiterInlineRenderer());
            ObjectRenderers.Add(new EmphasisInlineRenderer());
            ObjectRenderers.Add(new LineBreakInlineRenderer());
            ObjectRenderers.Add(new HtmlInlineRenderer());
            ObjectRenderers.Add(new HtmlEntityInlineRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());
        }

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XmlDocRender WriteEscape(string? content) => Write(WebUtility.HtmlEncode(content));

        /// <summary>
        /// Writes the content escaped for HTML.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="softEscape">Only escape &lt; and &amp;</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XmlDocRender WriteEscape(in StringSlice slice) => WriteEscape(slice.ToString());

        /// <summary>
        /// Writes the URL escaped for HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This instance</returns>
        public XmlDocRender WriteEscapeUrl(string? content)
        {
            if (content == null)
            { return this; }

            if (BaseUrl != null
                // According to https://github.com/dotnet/runtime/issues/22718
                // this is the proper cross-platform way to check whether a uri is absolute or not:
                && Uri.TryCreate(content, UriKind.RelativeOrAbsolute, out var contentUri) && !contentUri.IsAbsoluteUri)
            {
                content = new Uri(BaseUrl, contentUri).AbsoluteUri;
            }

            return WriteEscape(content);
        }

        /// <summary>
        /// Writes the lines of a <see cref="LeafBlock"/>
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <returns>This instance</returns>
        public XmlDocRender WriteLeafRawLines(LeafBlock leafBlock)
        {
            ArgumentNullException.ThrowIfNull(leafBlock);
            if (leafBlock.Lines.Lines is [StringLine _first, ..] lines)
            {
                _ = WriteEscape(_first.Slice);
                for (int i = 1; i < lines.Length; i++)
                {
                    _ = Write("<br/>").WriteEscape(lines[i].Slice);
                }
            }
            return this;
        }

        public XmlDocRender Write(in DefaultInterpolatedStringHandler handler)
        {
            Write(handler.Text);
            handler.Clear();
            return this;
        }
    }
}
