using System.Diagnostics;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This abstract class is a common root class for predefined performance counter categories.
    /// </summary>
    public abstract class PredefinedPmcCategory: PmcCategoryDefinitionBase
    {
        protected PredefinedPmcCategory()
        {
            IsPredefined = true;
        }
    }

    [PmcCategoryName("Processor")]
    [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
    public sealed class ProcessorCategory: PredefinedPmcCategory { }

    [PmcName("% Processor Time")]
    [PmcDefaultInstance("_Total")]
    [PmcPredefined]
    public sealed class ProcessorTimePercentagePmc: PmcDefinitionBase<ProcessorCategory> { }

    [PmcName("% Privileged Time")]
    [PmcDefaultInstance("_Total")]
    [PmcPredefined]
    public sealed class PrivilegedTimePercentagePmc : PmcDefinitionBase<ProcessorCategory> { }

    /// <summary>
    /// This class describes the performance counter category describing task counters.
    /// </summary>
    [PmcCategoryName("Seemplest Framework Counters")]
    [PmcCategoryHelp("This category contains counters related to Seemplest Framework.")]
    [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
    [PmcDefaultInstance("_Total")]
    public sealed class FrameworkPmcCategory : PmcCategoryDefinitionBase { }

    /// <summary>
    /// This counter represents the number of service calls.
    /// </summary>
    [PmcName("#of calls")]
    [PmcHelp("Represents the #of calls in regard of the operation named by the instance.")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    [PmcDefaultInstance("_Total")]
    public sealed class NumberOfCallsPmc : PmcDefinitionBase<FrameworkPmcCategory> { }

    /// <summary>
    /// This counter represents the number of service calls.
    /// </summary>
    [PmcName("#of calls in progress")]
    [PmcHelp("Represents the #of calls currently in progress.")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    [PmcDefaultInstance("_Total")]
    public sealed class CurrentCallsPmc : PmcDefinitionBase<FrameworkPmcCategory> { }

    /// <summary>
    /// This counter represents the number of service calls per one second.
    /// </summary>
    [PmcName("#of calls/second")]
    [PmcHelp("Represents the #of calls per one second in regard of the operation named by the instance.")]
    [PmcType(PerformanceCounterType.RateOfCountsPerSecond32)]
    [PmcDefaultInstance("_Total")]
    public sealed class NumberOfCallsPerSecondsPmc : PmcDefinitionBase<FrameworkPmcCategory> { }

    /// <summary>
    /// This counter represents the number of service calls failed.
    /// </summary>
    [PmcName("#of calls failed")]
    [PmcHelp("Represents the #of failed calls in regard of the operation named by the instance.")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    [PmcDefaultInstance("_Total")]
    public sealed class NumberOfFailedCallsPmc : PmcDefinitionBase<FrameworkPmcCategory> { }

    /// <summary>
    /// This counter represents the number of service calls failed per one second.
    /// </summary>
    [PmcName("#of calls failed/second")]
    [PmcHelp("Represents the #of failed calls per one second in regard of the operation named by the instance.")]
    [PmcType(PerformanceCounterType.RateOfCountsPerSecond32)]
    [PmcDefaultInstance("_Total")]
    public sealed class NumberOfFailedCallsPerSecondsPmc : PmcDefinitionBase<FrameworkPmcCategory> { }

    /// <summary>
    /// This counter represents the time of the last method executed.
    /// </summary>
    [PmcName("Last call execution time")]
    [PmcHelp("Number of milliseconds the last executed call took")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    [PmcDefaultInstance("_Total")]
    public sealed class LastCallTimePmc : PmcDefinitionBase<FrameworkPmcCategory> { }
}