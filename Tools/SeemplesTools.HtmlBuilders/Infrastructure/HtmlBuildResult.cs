using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This build result represents the result coming from building an IHtmlBuildable
    /// </summary>
    public class HtmlBuildResult : IBuildResult
    {
        private readonly HtmlHelper _htmlHelper;
        private readonly IHtmlBuildable _buildable;

        /// <summary>
        /// Represents an empty build result
        /// </summary>
        public static HtmlBuildResult Empty = new EmptyBuildResult();

        /// <summary>
        /// Creates a build result thar uses the specified HtmlHelper for its output
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to use</param>
        /// <param name="buildable">The buildable object</param>
        public HtmlBuildResult(HtmlHelper htmlHelper, IHtmlBuildable buildable)
        {
            _htmlHelper = htmlHelper;
            _buildable = buildable;
        }

        /// <summary>
        /// The result of the builder
        /// </summary>
        public virtual MvcHtmlString Result
        {
            get
            {
                var segments = _buildable.Render();
                segments.AddRange(_buildable.Complete());
                return FormatSegments(segments);
            }
        }

        /// <summary>
        /// Renders the result to the current output
        /// </summary>
        public virtual void Render()
        {
            _htmlHelper.ViewContext.Writer.Write(Result);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Result.ToString();
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

        private class EmptyBuildResult : HtmlBuildResult
        {
            /// <summary>
            /// Do not allow intantiation
            /// </summary>
            public EmptyBuildResult(): base(null, null)
            {
            }

            /// <summary>
            /// The result of the builder
            /// </summary>
            public override MvcHtmlString Result
            {
                get { return MvcHtmlString.Empty; }
            }

            /// <summary>
            /// Renders the result to the current output
            /// </summary>
            public override void Render()
            {
            }
        }
    }
}