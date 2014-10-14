using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class implements a "requited" validation arttribute that works with Angular code generation
    /// </summary>
    public class NgEmailAddressAttribute : DataTypeAttribute, IMetadataAware
    {
        // --- This attribute provides server-side email validation equivalent to jquery validate,
        // --- and therefore shares the same regular expression.
        private static readonly Regex s_EmailRegex = 
            new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public NgEmailAddressAttribute()
            : base(DataType.EmailAddress)
        {
        }

        /// <summary>
        /// Checks that the value of the data field is valid.
        /// </summary>
        /// <returns>
        /// The result of validation
        /// </returns>
        /// <param name="value">The data field value to validate.</param>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            var valueAsString = value as string;
            return valueAsString != null && s_EmailRegex.Match(valueAsString).Length > 0;
        }

        /// <summary>
        /// When implemented in a class, provides metadata to the model metadata creation process.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "email";
            var additionalMetadataValue = new ValidationAttributeMetadata(ATTR, ErrorMessageString);
            metadata.AdditionalValues.Add(ATTR, additionalMetadataValue);
        }
    }
}