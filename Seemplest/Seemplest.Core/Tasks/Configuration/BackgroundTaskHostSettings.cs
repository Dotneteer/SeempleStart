using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Seemplest.Core.Common;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.Tasks.Configuration
{
    /// <summary>
    /// This class describes the configuration settings for the background task host.
    /// </summary>
    public class BackgroundTaskHostSettings : ConfigurationSettingsBase
    {
        private const string DEFAULT_CONTEXT = "DefaultContext";
        private const string TASK_PROCESSORS = "TaskProcessors";

        private TaskExecutionContextSettings _defaultContext;
        private readonly List<TaskProcessorSettings> _taskProcessors =
            new List<TaskProcessorSettings>();

        /// <summary>
        /// The default name of these settings' root element
        /// </summary>
        public const string ROOT = "BackgroundTaskHost";

        /// <summary>
        /// Gets the default execution context of task processors
        /// </summary>
        public TaskExecutionContextSettings DefaultContext
        {
            get { return _defaultContext.Clone(); }
            private set { _defaultContext = value; }
        }

        /// <summary>
        /// Gets the list of task processor settings managed by this host
        /// </summary>
        public List<TaskProcessorSettings> GetTaskProcessors()
        {
            return _taskProcessors.Select(processor => processor.Clone()).ToList();
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public BackgroundTaskHostSettings()
        {
        }

        /// <summary>
        /// Creates a new instance with the specified settings
        /// </summary>
        /// <param name="context">Default execution context</param>
        /// <param name="processors">Task processor settings</param>
        public BackgroundTaskHostSettings(TaskExecutionContextSettings context, IEnumerable<TaskProcessorSettings> processors)
        {
            DefaultContext = context;
            _taskProcessors = new List<TaskProcessorSettings>(processors);
        }

        /// <summary>
        /// Creates a new instance of this class from the specified XML element
        /// </summary>
        /// <param name="element">XML element containg the settings</param>
        public BackgroundTaskHostSettings(XElement element)
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
            return new XElement(rootElement,
                DefaultContext.WriteToXml(DEFAULT_CONTEXT),
                new XElement(TASK_PROCESSORS,
                    from processor in _taskProcessors
                    select processor.WriteToXml(TaskProcessorSettings.ROOT)));
        }

        /// <summary>
        /// Parse the specified configuration settings
        /// </summary>
        /// <param name="element">Element holding configuration settings</param>
        protected override void ParseFromXml(XElement element)
        {
            element.ProcessOptionalElement(DEFAULT_CONTEXT,
                item => { DefaultContext = new TaskExecutionContextSettings(item); });
            element.ProcessContainerItems(TASK_PROCESSORS, TaskProcessorSettings.ROOT,
                                          item => _taskProcessors.Add(new TaskProcessorSettings(item)));
        }

        /// <summary>
        /// Creates a clone of this setting instance.
        /// </summary>
        /// <returns>Cloned instance</returns>
        public BackgroundTaskHostSettings Clone()
        {
            return Clone<BackgroundTaskHostSettings>();
        }
    }
}