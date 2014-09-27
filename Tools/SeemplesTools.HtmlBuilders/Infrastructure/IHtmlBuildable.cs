using System.Collections.Generic;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This interface represents a node in the HTML markup that can be
    /// built by an IHtmlBuilder.
    /// </summary>
    public interface IHtmlBuildable
    {
        /// <summary>
        /// Sets the builder context this element can be build within
        /// </summary>
        /// <param name="context">Builder context</param>
        void SetContext(IHtmlBuilderContext context);

        /// <summary>
        /// Depth of the buildable in the chain of buildable elements
        /// </summary>
        /// <remarks>
        /// This property can be used for indentation when rendering the output
        /// </remarks>
        int Depth { get; set; }

        /// <summary>
        /// Renders the HTML buildable object
        /// </summary>
        /// <returns>A list of HTML encoded strings</returns>
        /// <remarks>
        /// This method should render the start tag, and the nested content of the HTML buildable,
        /// optionally may render the closing tag.
        /// The result of the rendering is a list where each item of the list can be formatted separately
        /// (indented and written into a separate line).
        /// </remarks>
        List<RenderedSegment> Render();

        /// <summary>
        /// Renders the (optional) closing tag of the buildable object
        /// </summary>
        /// <returns>HTML encoded string</returns>
        /// <remarks>
        /// This tag should render the closing tag if and only if that Build() have not 
        /// already done it.
        /// The result of the rendering is a list where each item of the list can be formatted separately
        /// (indented and written into a separate line).
        /// </remarks>
        List<RenderedSegment> Complete();
    }
}