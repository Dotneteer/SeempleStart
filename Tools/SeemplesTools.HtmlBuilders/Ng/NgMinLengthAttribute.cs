using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class implements a "minimum length" validation arttribute that works with Angular code generation
    /// </summary>
    public class NgMinLengthAttribute : MinLengthAttribute, IMetadataAware
    {
        /// <summary>
        /// Initializes this attribute with the specified minimum length
        /// </summary>
        /// <param name="length"></param>
        public NgMinLengthAttribute(int length)
            : base(length)
        {
        }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var additionalMetadataValue = new ValidationAttributeMetadata("minlen", ErrorMessageString, 
                Length.ToString(CultureInfo.InvariantCulture));
            metadata.AdditionalValues.Add(Guid.NewGuid().ToString(), additionalMetadataValue);
        }
    }
}