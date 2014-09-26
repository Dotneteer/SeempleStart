using System.Diagnostics;
using Seemplest.Core.Configuration;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class represents an event when a task processor has been started.
    /// </summary>
    [EventType(EventLogEntryType.Information)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_PROCESSOR_STARTED_ID)]
    [EventMessage("Task processor started.")]
    public sealed class TaskProcessorStarted : LogEventBase
    { }

    /// <summary>
    /// This class represents an event when a task processor has been stopped.
    /// </summary>
    [EventType(EventLogEntryType.Information)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_PROCESSOR_STOPPED_ID)]
    [EventMessage("Task processor stopped.")]
    public sealed class TaskProcessorStopped : LogEventBase
    { }

    /// <summary>
    /// This class represents an event when a task processor has been stopped with timeout.
    /// </summary>
    [EventType(EventLogEntryType.Warning)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_PROCESSOR_STOPPED_WITH_TIMEOUT_ID)]
    [EventMessage("Task processor did not stop within timeout limits.")]
    public sealed class TaskProcessorStoppedWithTimeout : LogEventBase
    { }

    /// <summary>
    /// This class represents an event when a poisoning message is found in a queue.
    /// </summary>
    [EventType(EventLogEntryType.Warning)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.POISONING_MESSAGE_FOUND_ID)]
    [EventMessage("A poisoning message found in a queue")]
    public sealed class PoisoningMessageFound : LogEventBase
    {
    }

    /// <summary>
    /// This class represents an event when a task has been interrupted in a queue.
    /// </summary>
    [EventType(EventLogEntryType.Warning)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_EXECUTION_INTERRUPTED_ID)]
    [EventMessage("A task execution has been interrupted.")]
    public sealed class TaskExecutionInterrupted : LogEventBase
    {
    }

    /// <summary>
    /// This class represents an event task execution failed.
    /// </summary>
    [EventType(EventLogEntryType.Error)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_EXECUTION_FAILED_ID)]
    [EventMessage("Task execution failed.")]
    public sealed class TaskExecutionFailed : LogEventBase
    {
    }

    /// <summary>
    /// This class represents an event task processor host raised an exception.
    /// </summary>
    [EventType(EventLogEntryType.Error)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_PROCESSOR_HOST_EXCEPTION_ID)]
    [EventMessage("Task processor host failed.")]
    public sealed class TaskProcessorHostFailed : LogEventBase
    {
    }

    /// <summary>
    /// This class represents an event task processor host notifies a warning.
    /// </summary>
    [EventType(EventLogEntryType.Warning)]
    [EventLogName(ConfigurationConstants.CORE_LOG_NAME)]
    [EventSource(ConfigurationConstants.AZURE_COMPONENTS_SOURCE)]
    [EventCategoryId(ConfigurationConstants.CORE_CATEGORY)]
    [EventId(ConfigurationConstants.TASK_PROCESSOR_HOST_WARNING_ID)]
    [EventMessage("Task processor host raised a warning.")]
    public sealed class TaskProcessorHostWarning : LogEventBase
    {
    }
}