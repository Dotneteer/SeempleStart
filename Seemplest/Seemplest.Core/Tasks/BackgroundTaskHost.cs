using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Tasks.Configuration;
using Seemplest.Core.WindowsEventLog;

namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This class is responsible for managing background task processors
    /// </summary>
    public class BackgroundTaskHost : IDisposable
    {
        /// <summary>
        /// Maximum number of instances that can be run by a single background task host
        /// </summary>
        public const int MAX_INSTANCES = 256;

        private readonly TaskProcessorInstancesCollection _processors = new TaskProcessorInstancesCollection();
        private BackgroundTaskHostSettings _configuration;

        /// <summary>
        /// Gets the configuration of the background task host
        /// </summary>
        public BackgroundTaskHostSettings Configuration
        {
            get { return _configuration.Clone(); }
            private set { _configuration = value; }
        }

        /// <summary>
        /// Gets the maximum number of instances
        /// </summary>
        public int MaxInstances { get; private set; }

        /// <summary>
        /// Gets the flag indicating that the host has been already started.
        /// </summary>
        public bool HasStarted { get; private set; }

        /// <summary>
        /// Gets the collection of processor instances
        /// </summary>
        public TaskProcessorInstancesCollection ProcessorInstances
        {
            get { return _processors; }
        }

        /// <summary>
        /// Creates a new instance of this class using the specified configuration.
        /// </summary>
        /// <param name="configuration">Host configuration</param>
        /// <param name="maxInstances">Maximum number of instances allowed</param>
        public BackgroundTaskHost(BackgroundTaskHostSettings configuration, int maxInstances = MAX_INSTANCES)
        {
            Configuration = configuration;
            MaxInstances = maxInstances;
        }

        /// <summary>
        /// Creates a new instance of this class using the configuration specified
        /// in the passed XML element
        /// </summary>
        /// <param name="element">Configuration element</param>
        public BackgroundTaskHost(XElement element)
        {
            Configuration = new BackgroundTaskHostSettings(element);
        }

        /// <summary>
        /// Changes the current configuration of background tasks.
        /// </summary>
        /// <param name="configuration">New configuration</param>
        /// <param name="maxInstances">Number of new instances</param>
        public void Reconfigure(BackgroundTaskHostSettings configuration, int maxInstances = MAX_INSTANCES)
        {
            var wasInProgress = HasStarted;
            if (HasStarted) Stop();
            Configuration = configuration;
            MaxInstances = maxInstances;
            if (wasInProgress) Start();
        }

        /// <summary>
        /// Starts the background task host
        /// </summary>
        public void Start()
        {
            if (HasStarted) return;
            HasStarted = true;
            PrepareInstances();
            try
            {
                foreach (var processor in _processors)
                {
                    foreach (var instance in processor.Instances)
                    {
                        instance.Name = processor.Name;
                        instance.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                WindowsEventLogger.Log<TaskProcessorHostFailed>(
                    String.Format("Task processor host failed to start instances with the following error: {0}", ex));
            }
        }

        /// <summary>
        /// Stops the background task host
        /// </summary>
        public void Stop()
        {
            if (!HasStarted) return;
            HasStarted = false;
            try
            {
                foreach (var instance in _processors.SelectMany(processor => processor.Instances))
                {
                    instance.Stop();
                }
            }
            catch (Exception ex)
            {
                WindowsEventLogger.Log<TaskProcessorHostFailed>(
                    String.Format("Task processor host failed to stop instances with the following error: {0}", ex));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            foreach (var instance in _processors.SelectMany(processor => processor.Instances))
            {
                instance.Dispose();
            }
        }

        /// <summary>
        /// Prepares all task processor instances to run
        /// </summary>
        private void PrepareInstances()
        {
            _processors.Clear();
            try
            {
                var instancesCreated = 0;
                var defaultContext = new DefaultTaskExecutionContext(Configuration.DefaultContext);
                foreach (var taskProcessor in Configuration.GetTaskProcessors())
                {
                    var processorType = GetProcessorType(taskProcessor);
                    var instancesToCreate = taskProcessor.InstanceCount;
                    if (instancesCreated + instancesToCreate > MAX_INSTANCES)
                    {
                        instancesToCreate = MAX_INSTANCES - instancesCreated;
                        if (instancesToCreate < 0) instancesToCreate = 0;
                        string message;
                        if (instancesToCreate == 0)
                        {
                            message = String.Format(
                                "No instances has been created for '{0}' task processor, because " +
                                "otherwise the maximum amount of {1}' instances would be exceeded.",
                                taskProcessor.Name, MaxInstances);
                        }
                        else
                        {
                            message = String.Format(
                                "Only {0} instances has been created for '{1}' task processor, because " +
                                "otherwise the maximum amount of {2}' instances would be exceeded.",
                                instancesToCreate, taskProcessor.Name, MaxInstances);
                        }
                        WindowsEventLogger.Log<TaskProcessorHostWarning>(message);
                    }
                    var processorInfo = new TaskProcessorInstances(taskProcessor.Name);
                    _processors.Add(processorInfo);
                    for (var i = 0; i < instancesToCreate; i++)
                    {
                        // --- Create the processor instance and set its properties
                        var instance = (ITaskProcessor)ConfigurationHelper
                                                            .PrepareInstance(processorType,
                                                                             taskProcessor.Properties);
                        var context = taskProcessor.Context == null
                                          ? defaultContext
                                          : new DefaultTaskExecutionContext(taskProcessor.Context);
                        instance.SetContext(context);
                        processorInfo.Instances.Add(instance);

                        // --- Set the peek policy
                        var queuedInstance = instance as IQueuedTaskProcessor;
                        if (queuedInstance != null && taskProcessor.PeekPolicyType != null)
                        {
                            queuedInstance.PeekPolicy = Activator.CreateInstance(taskProcessor.PeekPolicyType)
                                as IQueuePeekPolicy;
                            if (queuedInstance.PeekPolicy != null)
                            {
                                queuedInstance.PeekPolicy.SetTaskExecutionContext(context);
                            }
                        }
                        instancesCreated++;
                    }
                }
            }
            catch (Exception ex)
            {
                WindowsEventLogger.Log<TaskProcessorHostFailed>(
                    String.Format("Task processor host failed to prepare instances with the following error: {0}", ex));
            }
        }

        /// <summary>
        /// Gets the type implementing the specified task processor
        /// </summary>
        /// <param name="taskProcessor">Task processor information</param>
        private static Type GetProcessorType(TaskProcessorSettings taskProcessor)
        {
            switch (taskProcessor.ProcessorType)
            {
                case TaskProcessorSettings.CONTINUOUS_TYPE:
                    return typeof(ContinuousTaskProcessor<>).MakeGenericType(taskProcessor.TaskType);
                case TaskProcessorSettings.SCHEDULED_TYPE:
                    return typeof(ScheduledTaskProcessor<>).MakeGenericType(taskProcessor.TaskType);
                case TaskProcessorSettings.QUEUED_TYPE:
                    return typeof(QueuedTaskProcessor<,>).MakeGenericType(taskProcessor.TaskType,
                        taskProcessor.ArgumentType);
                case TaskProcessorSettings.DOUBLE_QUEUED:
                    return typeof(QueuedTaskProcessor<,,>).MakeGenericType(taskProcessor.TaskType,
                        taskProcessor.ArgumentType, taskProcessor.ResultType);
                default:
                    throw new ConfigurationErrorsException(String.Format("ProcessorType must be one of the " +
                        "following values: '{0}', '{1}', {2} or '{3}'",
                        TaskProcessorSettings.SCHEDULED_TYPE,
                        TaskProcessorSettings.CONTINUOUS_TYPE,
                        TaskProcessorSettings.QUEUED_TYPE,
                        TaskProcessorSettings.DOUBLE_QUEUED));
            }
        }

        /// <summary>
        /// Provides information about task processor instances
        /// </summary>
        public struct TaskProcessorInstances
        {
            public readonly string Name;
            public readonly List<ITaskProcessor> Instances;

            public TaskProcessorInstances(string name)
                : this()
            {
                Name = name;
                Instances = new List<ITaskProcessor>();
            }
        }

        /// <summary>
        /// Stores the instances of task processors
        /// </summary>
        public class TaskProcessorInstancesCollection : KeyedCollection<string, TaskProcessorInstances>
        {
            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <returns>
            /// The key for the specified element.
            /// </returns>
            /// <param name="item">The element from which to extract the key.</param>
            protected override string GetKeyForItem(TaskProcessorInstances item)
            {
                return item.Name;
            }
        }
    }
}