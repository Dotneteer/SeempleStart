using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class is intended to be the common base class of all HTML 
    /// buildable types
    /// </summary>
    public abstract class HtmlBuildableBase : IHtmlBuildable
    {
        /// <summary>
        /// Void HTML elements
        /// </summary>
        protected static readonly List<string> VoidElements = new List<string>
        {
            "area",
            "base",
            "br",
            "hr",
            "img",
            "input",
            "meta",
            "param",
            "source",
            "track"
        };

        /// <summary>
        /// Gets the builder that provides the context for this buildable
        /// element.
        /// </summary>
        public IHtmlBuilderContext ParentContext { get; private set; }

        /// <summary>
        /// Sets the builder context this element can be build within
        /// </summary>
        /// <param name="context">Builder context</param>
        public void SetContext(IHtmlBuilderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            ParentContext = context;
            Depth = context.Depth;
            OnBuilderContextAttached();
        }

        /// <summary>
        /// Sets up the HTML element after it has been attached to its builder
        /// </summary>
        protected virtual void OnBuilderContextAttached()
        {
        }

        /// <summary>
        /// Depth of the buildable in the chain of buildable elements
        /// </summary>
        /// <remarks>
        /// This property can be used for indentation when rendering the output
        /// </remarks>
        public int Depth { get; set; }

        /// <summary>
        /// Renders the HTML markup node
        /// </summary>
        /// <returns>HTML encoded string</returns>
        public abstract List<RenderedSegment> Render();

        /// <summary>
        /// Renders the (optional) closing tag of the buildable object
        /// </summary>
        /// <returns>HTML encoded string</returns>
        /// <remarks>
        /// This tag should render the closing tag if and only if that Build() have not 
        /// already done it.
        /// </remarks>
        public abstract List<RenderedSegment> Complete();

        /// <summary>
        /// Renders the full markup of the buildable object
        /// </summary>
        public MvcHtmlString Markup
        {
            get
            {
                var segments = Render();
                segments.AddRange(Complete());
                return FormatSegments(segments);
            }
        }

        /// <summary>
        /// Formats the specified segments into an MvcHtmlString instance
        /// </summary>
        /// <param name="segments">Segments to format</param>
        /// <returns>MvcHtmlString representation</returns>
        public static MvcHtmlString FormatSegments(IEnumerable<RenderedSegment> segments)
        {
            var output = new StringBuilder();
            foreach (var segment in segments)
            {
                if (HtmlBuilderConfiguration.UseLineBreaks)
                {
                    var line = string.Format("{0}{1}",
                        new String(HtmlBuilderConfiguration.IndentChar,
                            HtmlBuilderConfiguration.IndentWidth * (segment.Depth)),
                        segment.Segment);
                    output.AppendLine(line);
                }
                else
                {
                    output.Append(segment.Segment);
                }
            }
            return new MvcHtmlString(output.ToString());
        }
    }
}