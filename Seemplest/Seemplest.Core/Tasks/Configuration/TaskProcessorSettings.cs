using System;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Tasks.Configuration
{
    /// <summary>
    /// This class defines the configuration settings of a task processor
    /// </summary>
    public class TaskProcessorSettings : ConfigurationSettingsBase
    {
        private const string PROCESSOR_TYPE = "processorType";
        private const string TASK_TYPE = "taskType";
        private const string ARGUMENT_TYPE = "argumentType";
        private const string RESULT_TYPE = "resultType";
        private const string PEEK_POLICY = "peekPolicy";
        private const string INSTANCE_COUNT = "instanceCount";
        private const string EXECUTION_CONTEXT = "Context";
        private const string PROPERTIES = "ProcessorProperties";
        private const string NAME = "name";

        private readonly PropertySettingsCollection _propertysSettings = new PropertySettingsCollection();

        /// <summary>
        /// Continuous task processor name
        /// </summary>
        public const string CONTINUOUS_TYPE = "continuous";

        /// <summary>
        /// Scheduled task processor name
        /// </summary>
        public const string SCHEDULED_TYPE = "scheduled";

        /// <summary>
        /// Queued task processor name
        /// </summary>
        public const string QUEUED_TYPE = "queued";

        /// <summary>
        /// Double queued task processor name
        /// </summary>
        public const string DOUBLE_QUEUED = "queuedWithResult";

        /// <summary>
        /// Gets the root element name of these settings
        /// </summary>
        public const string ROOT = "TaskProcessor";

        /// <summary>
        /// Gets the name of the task processor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the type of task processor
        /// </summary>
        public string ProcessorType { get; set; }

        /// <summary>
        /// Gets the type of task managed by the processor
        /// </summary>
        public Type TaskType { get; set; }

        /// <summary>
        /// Gets the type of task argument
        /// </summary>
        public Type ArgumentType { get; set; }

        /// <summary>
        /// Gets the type of task result
        /// </summary>
        public Type ResultType { get; set; }

        /// <summary>
        /// Gets the type of peek policy
        /// </summary>
        public Type PeekPolicyType { get; set; }

        /// <summary>
        /// Gets the number of instances to be used by this processor
        /// </summary>
        public int InstanceCount { get; set; }

        /// <summary>
        /// Gets the execution context belonging to the processor
        /// </summary>
        public TaskExecutionContextSettings Context { get; set; }

        /// <summary>
        /// Gets the property dictionary of the execution context
        /// </summary>
        public PropertySettingsCollection Properties
        {
            get { return _propertysSettings; }
        }

        /// <summary>
        /// Creates a new instance of this class form the specified parameters.
        /// </summary>
        /// <param name="name">Task processor name</param>
        /// <param name="processorType">Processor type</param>
        /// <param name="taskType">Task type</param>
        /// <param name="instanceCount">Number of instances</param>
        /// <param name="context">Task execution copntext</param>
        /// <param name="properties">Collection of properties</param>
        public TaskProcessorSettings(string name, string processorType, Type taskType, int instanceCount,
            TaskExecutionContextSettings context, PropertySettingsCollection properties = null)
        {
            Name = name;
            ProcessorType = processorType;
            TaskType = taskType;
            InstanceCount = instanceCount;
            Context = context;
            if (properties != null)
            {
                _propertysSettings = properties;
            }
        }

        /// <summary>
        /// Creates a new instance from the specified XML element
        /// </summary>
        /// <param name="element"></param>
        public TaskProcessorSettings(XElement element)
        {
            ReadFromXml(element);
        }

        /// <summary>
        /// Writes the object to an XElement instance using the specified root element name.
        /// </summary>
        /// <param name="rootElement">Root element name</param>
        /// <returns>XElement representation of the object</returns>
        public override XElement WriteToXml(XName rootElement)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return new XElement(
                rootElement,
                new XAttribute(NAME, Name),
                ProcessorType == null ? null : new XAttribute(PROCESSOR_TYPE, ProcessorType),
                TaskType == null ? null : new XAttribute(TASK_TYPE, TaskType.AssemblyQualifiedName),
                ArgumentType == null ? null : new XAttribute(ARGUMENT_TYPE, ArgumentType.AssemblyQualifiedName),
                ResultType == null ? null : new XAttribute(RESULT_TYPE, ResultType.AssemblyQualifiedName),
                PeekPolicyType == null ? null : new XAttribute(PEEK_POLICY, PeekPolicyType.AssemblyQualifiedName),
                new XAttribute(INSTANCE_COUNT, InstanceCount),
                Context == null ? null : Context.WriteToXml(EXECUTION_CONTEXT),
                _propertysSettings.Count == 0 ? null : _propertysSettings.WriteToXml(PROPERTIES));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            Name = element.NonWhiteSpaceStringAttribute(NAME);
            ProcessorType = element.NonWhiteSpaceStringAttribute(PROCESSOR_TYPE);
            TaskType = element.TypeAttribute(TASK_TYPE);
            ArgumentType = element.OptionalTypeAttribute(ARGUMENT_TYPE);
            ResultType = element.OptionalTypeAttribute(RESULT_TYPE);
            PeekPolicyType = element.OptionalTypeAttribute(PEEK_POLICY);
            InstanceCount = element.OptionalIntAttribute(INSTANCE_COUNT, 1);
            element.ProcessOptionalElement(EXECUTION_CONTEXT,
                item => Context = new TaskExecutionContextSettings(item));
            element.ProcessOptionalElement(PROPERTIES, item => _propertysSettings.ReadFromXml(item));
        }

        /// <summary>
        /// Creates a clone of this setting instance.
        /// </summary>
        /// <returns>Cloned instance</returns>
        public TaskProcessorSettings Clone()
        {
            return Clone<TaskProcessorSettings>();
        }
    }
}