using System.Collections.Generic;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This class allows you to create a Bootstrap-aware HTML element with
    /// a tag name specified at construction time
    /// </summary>
    public class BsHtmlElement : BsHtmlElementBase<BsHtmlElement>
    {
        /// <summary>
        /// Initializes a new instance with the specified tag name
        /// </summary>
        /// <param name="tag">Tag name</param>
        public BsHtmlElement(string tag)
            : base(tag)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public BsHtmlElement(string tag, IDictionary<string, object> attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public BsHtmlElement(string tag, params object[] attribs)
            : base(tag, attribs)
        {
        }
    }
}