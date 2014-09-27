using System;
using System.Web.ModelBinding;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This attribute can be used as data annotation to specify the control width of
    /// the corresponding property
    /// </summary>
    public class BsWidthAttribute: Attribute, IMetadataAware
    {
        /// <summary>
        /// Width on extra small device
        /// </summary>
        public int? Xs { get; set; }

        /// <summary>
        /// Width on small device
        /// </summary>
        public int? Sm { get; set; }

        /// <summary>
        /// Width on medium small device
        /// </summary>
        public int? Md { get; set; }

        /// <summary>
        /// Width on large device
        /// </summary>
        public int? Lg { get; set; }
        
        /// <summary>
        /// Enables metadata-aware attributes to perform required processing of metadata after the metadata is created.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            const string ATTR = "bsWidth";
            metadata.AdditionalValues.Add(ATTR, this);
        }
    }
}