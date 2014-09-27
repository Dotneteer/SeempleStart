using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    public interface IHtmlBuilderContext
    {
        /// <summary>
        /// Provides access to the parent builder that may have extra context 
        /// information 
        /// </summary>
        IHtmlBuilderContext ParentBuilderContext { get; }

        /// <summary>
        /// Depth of the builder in the builder chain
        /// </summary>
        /// <remarks>
        /// This property can be used for indentation when rendering the output
        /// </remarks>
        int Depth { get; }

        /// <summary>
        /// Provides the context the builder uses when building the elements
        /// </summary>
        HtmlHelper HtmlHelper { get; }

        /// <summary>
        /// Sets the value of the specified property
        /// </summary>
        /// <param name="propName">Property name</param>
        /// <param name="value">Property value</param>
        /// <remarks>Setting the property value to null removes the property</remarks>
        void SetProperty(string propName, object value);

        /// <summary>
        /// Gets the specified property from the context of the builder
        /// </summary>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="propName">Name of the property</param>
        /// <param name="value">Property value, if found</param>
        /// <returns>True, if the property value has been found; otherwise, false</returns>
        /// <remarks>
        /// When the property cannot be found at this builder, the search goes on the 
        /// chain of parent builders. 
        /// </remarks>
        bool GetProperty<TProp>(string propName, out TProp value);
    }
}