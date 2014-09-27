using System.Collections.Generic;
using System.Linq;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class defines additional metadata for custom validation attributes
    /// </summary>
    public sealed class ValidationAttributeMetadata
    {
        /// <summary>
        /// Initializes an instance with custom validation attributes
        /// </summary>
        /// <param name="directiveName">Validation Angular directive</param>
        /// <param name="errorMessage">Validation Angular error message</param>
        /// <param name="attributeValue">Value of the Angular directive attribute</param>
        /// <param name="additionalAttributes">Additional HTML attributes to render</param>
        public ValidationAttributeMetadata(string directiveName, string errorMessage, 
            string attributeValue = "", 
            Dictionary<string, string> additionalAttributes = null)
        {
            DirectiveSet = new Dictionary<string, string> {{directiveName, attributeValue}};
            ErrorMessage = errorMessage;
            AdditionalAttributes = additionalAttributes ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes an instance with custom validation attributes
        /// </summary>
        /// <param name="directiveSet">Set of directives to render</param>
        /// <param name="errorMessage">Validation Angular error message</param>
        /// <param name="additionalAttributes">Additional HTML attributes to render</param>
        public ValidationAttributeMetadata(Dictionary<string, string> directiveSet, string errorMessage, 
            Dictionary<string, string> additionalAttributes = null)
        {
            DirectiveSet = directiveSet;
            ErrorMessage = errorMessage;
            AdditionalAttributes = additionalAttributes;
        }

        /// <summary>
        /// Set of directives (name and value pairs)
        /// </summary>
        public Dictionary<string, string> DirectiveSet { get; private set; }

        /// <summary>
        /// Validation Angular directive
        /// </summary>
        public string DirectiveName
        {
            get { return DirectiveSet.Count == 1 ? DirectiveSet.Keys.First() : null; }
        }
        
        /// <summary>
        /// Validation Angular error message
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Value of the Angular directive attribute
        /// </summary>
        public Dictionary<string, string> AdditionalAttributes { get; set; }

        /// <summary>
        /// Additional HTML attributes to render
        /// </summary>
        public string AttributeValue
        {
            get { return DirectiveSet.Count == 1 ? DirectiveSet.Values.First() : null; }
        }
    }
}