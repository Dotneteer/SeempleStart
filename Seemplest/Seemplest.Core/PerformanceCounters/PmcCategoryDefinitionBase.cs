using System.Diagnostics;
using Seemplest.Core.Common;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class is an abstract base class for all performance counter category definitions
    /// </summary>
    public abstract class PmcCategoryDefinitionBase
    {
        /// <summary>
        /// Gets the category name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the category help text
        /// </summary>
        public string Help { get; protected set; }

        /// <summary>
        /// Gets the type of the category
        /// </summary>
        public PerformanceCounterCategoryType Type { get; protected set; }

        /// <summary>
        /// Gets the flag indicating if this category is predefined.
        /// </summary>
        public bool IsPredefined { get; protected set; }
        
        /// <summary>
        /// Creates a new instance according to attributes decorating the derived class
        /// </summary>
        protected PmcCategoryDefinitionBase()
        {
            var attrSet = new AttributeSet(GetType(), typeof (PmcAttributeBase));
            Name = attrSet.Single<PmcCategoryNameAttribute>().Value;
            Help = attrSet.Optional(new PmcCategoryHelpAttribute(string.Empty)).Value;
            Type = attrSet.Optional(new PmcCategoryTypeAttribute(PerformanceCounterCategoryType.Unknown)).Type;
            IsPredefined = attrSet.Optional(new PmcCategoryPredefinedAttribute(false)).IsPredefined;
        }
    }
}