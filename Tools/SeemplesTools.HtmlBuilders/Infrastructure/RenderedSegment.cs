using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class represents a rendered HTML segment that is preapred for formatting
    /// </summary>
    public class RenderedSegment
    {
        /// <summary>
        /// The segment that contains the string to send to the output
        /// </summary>
        public MvcHtmlString Segment { get; private set; }

        /// <summary>
        /// The depth of the segment
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Initializes the segment with the specified MvcHtmlString instance
        /// </summary>
        /// <param name="segment">Segment information</param>
        /// <param name="depth">Depth information</param>
        public RenderedSegment(MvcHtmlString segment, int depth = 0)
        {
            Segment = segment;
            Depth = depth;
        }

        /// <summary>
        /// Initializes the segment with the specified MvcHtmlString instance
        /// </summary>
        /// <param name="segment">Segment information</param>
        /// <param name="depth">Depth information</param>
        public RenderedSegment(string segment, int depth = 0)
        {
            Segment = new MvcHtmlString(segment);
            Depth = depth;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Segment.ToString();
        }
    }
}