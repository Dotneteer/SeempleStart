using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class checks whether the specified property can be used as a strong password
    /// </summary>
    public class NgStrongPasswordAttribute : DataTypeAttribute, IMetadataAware
    {
        public NgStrongPasswordAttribute() : base(DataType.Password)
        {
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        /// <param name="value">The value of the object to validate. </param>
        public override bool IsValid(object value)
        {
            var str = value as string;
            if (str == null || str.Length < 6) return false;
            var hasDigit = false;
            var hasNonLetterOrDigit = false;
            var hasLowerCase = false;
            var hasUpperCase = false;
            foreach (var c in str)
            {
                hasDigit |= Char.IsDigit(c);
                hasNonLetterOrDigit |= !Char.IsLetterOrDigit(c);
                hasLowerCase |= Char.IsLower(c);
                hasUpperCase |= Char.IsUpper(c);
            }
            return hasDigit && hasNonLetterOrDigit && hasLowerCase || hasUpperCase;
        }

        /// <summary>
        /// Enables metadata-aware attributes to perform required processing of metadata after the metadata is created.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "strongpsw";
            var additionalMetadataValue = new ValidationAttributeMetadata(ATTR, ErrorMessageString);
            metadata.AdditionalValues.Add(ATTR, additionalMetadataValue);
        }
    }
}