namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This interface defines the responsibility of a builder that can build HTML in a particular MVC context
    /// </summary>
    public interface IHtmlBuilder : IHtmlBuilderContext
    {
        /// <summary>
        /// This is the element that can be built by this builder.
        /// </summary>
        IHtmlBuildable Buildable { get; }

        /// <summary>
        /// Carries out all tasks related to the building process.
        /// </summary>
        /// <remarks>
        /// These activities should include rendering the start tag of a certain HTML element
        /// </remarks>
        void Build();

        /// <summary>
        /// Carries out all tasks related to completing the build process.
        /// </summary>
        /// <remarks>
        /// These activites may include rendering the end tag of a certain HTML element
        /// </remarks>
        void CompleteBuild();
    }
}