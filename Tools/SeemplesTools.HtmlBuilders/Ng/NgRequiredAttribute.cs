using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class implements a "requited" validation arttribute that works with Angular code generation
    /// </summary>
    public class NgRequiredAttribute : RequiredAttribute, IMetadataAware
    {
        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "required";
            var additionalMetadataValue = new ValidationAttributeMetadata(ATTR, ErrorMessageString);
            metadata.AdditionalValues.Add(ATTR, additionalMetadataValue);
        }
    }
}