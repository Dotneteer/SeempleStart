using System.Diagnostics;
using Seemplest.Core.PerformanceCounters;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class describes the performance counter category describing task counters.
    /// </summary>
    [PmcCategoryName("Seemplest Tasks")]
    [PmcCategoryHelp("This category contains counters that indicate how task processing is working on.")]
    [PmcCategoryType(PerformanceCounterCategoryType.MultiInstance)]
    public sealed class TaskProcessorPmcCategory : PmcCategoryDefinitionBase { }

    /// <summary>
    /// This counter represents the number of tasks processed by a particular task processor.
    /// </summary>
    [PmcName("#of Tasks Processed")]
    [PmcHelp("#of tasks processed by a particular task processor")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    public sealed class TasksProcessedPmc : PmcDefinitionBase<TaskProcessorPmcCategory> { }

    /// <summary>
    /// This counter represents the number of tasks processed by a particular task processor in one second.
    /// </summary>
    [PmcName("#of Tasks Processed/second")]
    [PmcHelp("#of tasks processed by a particular task processor per one second")]
    [PmcType(PerformanceCounterType.RateOfCountsPerSecond32)]
    public sealed class TasksProcessedPerSecondsPmc : PmcDefinitionBase<TaskProcessorPmcCategory> { }

    /// <summary>
    /// This counter represents the number of tasks failed while processed by a particular task processor.
    /// </summary>
    [PmcName("#of Tasks Failed")]
    [PmcHelp("#of tasks failed while processed by a particular task processor")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    public sealed class TasksFailedPmc : PmcDefinitionBase<TaskProcessorPmcCategory> { }

    /// <summary>
    /// This counter represents the number of tasks processed by a particular task processor in one second.
    /// </summary>
    [PmcName("#of Tasks Failed/second")]
    [PmcHelp("#of tasks failed while processed by a particular task processor per one second")]
    [PmcType(PerformanceCounterType.RateOfCountsPerSecond32)]
    public sealed class TasksFailedPerSecondsPmc : PmcDefinitionBase<TaskProcessorPmcCategory> { }

    /// <summary>
    /// This counter represents the number of tasks processed by a particular task processor.
    /// </summary>
    [PmcName("Last Task Processing Time")]
    [PmcHelp("Number of milliseconds the last executed task processing took")]
    [PmcType(PerformanceCounterType.NumberOfItems32)]
    public sealed class LastTaskProcessingTimePmc : PmcDefinitionBase<TaskProcessorPmcCategory> { }
}