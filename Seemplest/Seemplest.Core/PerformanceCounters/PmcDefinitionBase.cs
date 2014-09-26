using System;
using System.Diagnostics;
using Seemplest.Core.Common;

namespace Seemplest.Core.PerformanceCounters
{
    public abstract class PmcDefinitionBase
    {
        /// <summary>
        /// Gets the counter's category name
        /// </summary>
        public string CategoryName { get; protected set; }

        /// <summary>
        /// Gets the counter name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the flag indicating if this is a multi-instance counter
        /// </summary>
        public bool IsMultiInstance { get; protected set; }
        
        /// <summary>
        /// Gets the default instance name
        /// </summary>
        public string DefaultInstance { get; protected set; }

        /// <summary>
        /// Gets the counter help text
        /// </summary>
        public string Help { get; protected set; }

        /// <summary>
        /// Gets the type of the performance counter
        /// </summary>
        public PerformanceCounterType Type { get; protected set; }

        /// <summary>
        /// Gets the flag indicating if this counter is predefined.
        /// </summary>
        public bool IsPredefined { get; protected set; }
    }

    /// <summary>
    /// This class is an abstract base class for all performance counter category definitions
    /// </summary>
    public abstract class PmcDefinitionBase<TCat>: PmcDefinitionBase
        where TCat: PmcCategoryDefinitionBase, new()
    {
        /// <summary>
        /// Gets the type that represents the counter's category
        /// </summary>
        public Type Category { get; private set; }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        protected PmcDefinitionBase()
        {
            Category = typeof (TCat);
            CategoryName = new TCat().Name;
            var catAttrSet = new AttributeSet(typeof (TCat));
            IsMultiInstance =
                catAttrSet.Optional(new PmcCategoryTypeAttribute(PerformanceCounterCategoryType.Unknown)).Type ==
                PerformanceCounterCategoryType.MultiInstance;
            var attrSet = new AttributeSet(GetType(), typeof (PmcAttributeBase));
            Name = attrSet.Single<PmcNameAttribute>().Value;
            DefaultInstance = attrSet.Optional(new PmcDefaultInstanceAttribute(null)).Value;
            Help = attrSet.Optional(new PmcHelpAttribute(String.Empty)).Value;
            Type = attrSet.Optional(new PmcTypeAttribute(PerformanceCounterType.NumberOfItems32)).Type;
            IsPredefined = attrSet.Optional(new PmcPredefinedAttribute(false)).IsPredefined;
        }
    }
}