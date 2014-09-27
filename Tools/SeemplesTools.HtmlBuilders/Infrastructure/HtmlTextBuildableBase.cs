using System.Collections.Generic;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class represents an HTML text
    /// </summary>
    public abstract class HtmlTextBuildableBase : HtmlBuildableBase
    {
        /// <summary>
        /// Gets the related text
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Initializes the text with the specified text
        /// </summary>
        /// <param name="text"></param>
        protected HtmlTextBuildableBase(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Renders the (optional) closing tag of the buildable object
        /// </summary>
        /// <returns>HTML encoded string</returns>
        /// <remarks>
        /// This tag should render the closing tag if and only if that Build() have not 
        /// already done it.
        /// </remarks>
        public override List<RenderedSegment> Complete()
        {
            return new List<RenderedSegment>();
        }
    }
}