using System.Collections.Generic;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class represents an HTML comment
    /// </summary>
    public sealed class HtmlComment : HtmlTextBuildableBase
    {
        /// <summary>
        /// Initializes the text with the specified text
        /// </summary>
        /// <param name="text"></param>
        public HtmlComment(string text) : base(text)
        {
        }

        /// <summary>
        /// Renders the HTML markup node
        /// </summary>
        /// <returns>HTML encoded string</returns>
        public override List<RenderedSegment> Render()
        {
            return new List<RenderedSegment>
            {
                new RenderedSegment(string.Format("<!-- {0} -->", Text ?? ""), Depth)
            };
        }
    }
}