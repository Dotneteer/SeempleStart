using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class implements a "matches with" validation arttribute that works with Angular code generation
    /// </summary>
    public class NgMatchesWithAttribute : CompareAttribute, IMetadataAware
    {
        /// <summary>
        /// Initializes this attribute with the specified minimum length
        /// </summary>
        /// <param name="property">The name of the property to match</param>
        public NgMatchesWithAttribute(string property)
            : base(property)
        {
        }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "matches";
            var attrValue = "{{model." + OtherProperty + "}}";
            var additionalMetadataValue = new ValidationAttributeMetadata(ATTR, ErrorMessageString, attrValue);
            metadata.AdditionalValues.Add(ATTR, additionalMetadataValue);
        }
    }
}