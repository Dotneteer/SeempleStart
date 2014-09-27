using System.Collections.Generic;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class allows you to create an HTML element with a tag name
    /// specified at construction time
    /// </summary>
    public class HtmlElement: HtmlElementBase<HtmlElement>
    {
        /// <summary>
        /// Initializes a new instance with the specified tag name
        /// </summary>
        /// <param name="tag">Tag name</param>
        public HtmlElement(string tag)
            : base(tag)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public HtmlElement(string tag, IDictionary<string, object> attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public HtmlElement(string tag, params object[] attribs)
            : base(tag, attribs)
        {
        }
    }
}