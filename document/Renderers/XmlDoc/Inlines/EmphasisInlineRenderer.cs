using Markdig.Syntax.Inlines;

namespace Detours.Win32Metadata.Document.Renderers.XmlDoc.Inlines
{
    /// <summary>
    /// A XmlDoc renderer for an <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="XmlDocObjectRenderer{EmphasisInline}" />
    public class EmphasisInlineRenderer : XmlDocObjectRenderer<EmphasisInline>
    {
        protected override void Write(XmlDocRender renderer, EmphasisInline obj)
        {
            string[] tag = GetDefaultTag(obj);

            foreach (string t in tag)
            { _ = renderer.Write($"<{t}>"); }

            renderer.WriteChildren(obj);

            foreach (string t in tag.Reverse())
            { _ = renderer.Write($"</{t}>"); }
        }

        /// <summary>
        /// Gets the default HTML tag for ** and __ emphasis.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string[] GetDefaultTag(EmphasisInline obj) => obj.DelimiterChar switch
        {
            '*' or '_' => (obj.DelimiterCount % 4) switch
            {
                1 => ["i"],
                2 => ["b"],
                3 => ["b", "i"],
                0 or _ => [],
            },
            '+' when obj.DelimiterCount == 2 => ["u"],
            _ => [],
        };
    }
}
