using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This interface represents the result of a builder.
    /// </summary>
    public interface IBuildResult
    {
        /// <summary>
        /// The result of the builder
        /// </summary>
        MvcHtmlString Result { get; }

        /// <summary>
        /// Renders the result to the current output
        /// </summary>
        void Render();
    }
}