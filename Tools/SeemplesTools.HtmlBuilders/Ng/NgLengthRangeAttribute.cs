using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class implements a "minimum length" validation arttribute that works with Angular code generation
    /// </summary>
    public class NgLengthRangeAttribute : StringLengthAttribute, IMetadataAware
    {
        /// <summary>
        /// Initializes this attribute with the specified minimum length
        /// </summary>
        /// <param name="minLength">Minimum string length</param>
        /// <param name="maxLength">Maximum string length</param>
        public NgLengthRangeAttribute(int minLength, int maxLength)
            : base(maxLength)
        {
            MinimumLength = minLength;
        }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "lenrange";
            var additionalMetadataValue = new ValidationAttributeMetadata(
                new Dictionary<string, string>
                {
                    { "minlen", MinimumLength.ToString(CultureInfo.InvariantCulture) },
                    { "maxlen", MaximumLength.ToString(CultureInfo.InvariantCulture) },
                }, ErrorMessageString);
            metadata.AdditionalValues.Add(ATTR, additionalMetadataValue);
        }
    }
}