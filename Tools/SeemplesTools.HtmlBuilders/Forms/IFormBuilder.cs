using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Forms
{
    /// <summary>
    /// This interface defines the behavior of a form builder.
    /// </summary>
    public interface IFormBuilder: IHtmlBuilderContext
    {
        /// <summary>
        /// Gets the Bootstrap form instance that is about to build
        /// </summary>
        IBsForm BsForm { get; }
    }
}