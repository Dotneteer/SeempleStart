using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Seemplest.Core.Common;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This class provides data for Windows event log source creation
    /// </summary>
    public class EventLogCreationData
    {
        private readonly List<EventSourceCreationData> _logSourceData = 
            new List<EventSourceCreationData>();

        /// <summary>
        /// Clears the collection of event log sources
        /// </summary>
        public void Clear()
        {
            _logSourceData.Clear();
        }

        /// <summary>
        /// Gets the collection of event log source creation data
        /// </summary>
        public IReadOnlyList<EventSourceCreationData> SourceCreationData
        {
            get { return new ReadOnlyCollection<EventSourceCreationData>(_logSourceData); }
        }

        /// <summary>
        /// Adds a new event log source entry to the collection using the specified log name and source name
        /// </summary>
        /// <param name="logName">Name of the event log</param>
        /// <param name="sourceName">Name of the event source</param>
        /// <param name="logNameMapper">Optional mapper for the log name</param>
        /// <param name="sourceNameMapper">Optional mapper for the log source</param>
        public void Add(string logName, string sourceName, INameMapper logNameMapper = null, 
            INameMapper sourceNameMapper = null)
        {
            if (logNameMapper != null) logName = logNameMapper.Map(logName);
            if (sourceNameMapper != null) sourceName = sourceNameMapper.Map(sourceName);
            if (!_logSourceData.Any(i => i.LogName == logName && i.Source == sourceName))
            {
                _logSourceData.Add(new EventSourceCreationData(sourceName, logName));
            }
        }

        /// <summary>
        /// Adds a new event log source entry to the collection using the specified log event type's metadata
        /// </summary>
        /// <param name="logEventType"></param>
        /// <param name="logNameMapper">Optional mapper for the log name</param>
        /// <param name="sourceNameMapper">Optional mapper for the log source</param>
        public void Add(Type logEventType, INameMapper logNameMapper = null, 
            INameMapper sourceNameMapper = null)
        {
            if (!logEventType.IsSubclassOf(typeof (LogEventBase)))
            {
                throw new InvalidOperationException(
                string.Format("Add must be called with a LogEventBase derived type. "
                    + "{0} is not an appropriate type.", logEventType));
            }

            var eventInstance = Activator.CreateInstance(logEventType) as LogEventBase;
            if (eventInstance != null)
            {
                Add(eventInstance.LogName, eventInstance.Source, logNameMapper, sourceNameMapper);
            }
        }

        /// <summary>
        /// Merges all Windows event log source types from the specified assembly with the 
        /// creation data.
        /// </summary>
        /// <param name="assembly">Assembly to scan for perfromance counter types</param>
        /// <param name="logNameMapper">Optional mapper for the log name</param>
        /// <param name="sourceNameMapper">Optional mapper for the log source</param>
        public void MergeSourcesFromAssembly(Assembly assembly, INameMapper logNameMapper = null, 
            INameMapper sourceNameMapper = null)
        {
            foreach (var type in assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(LogEventBase)) && 
                    type.GetCustomAttribute<SkipLogSourceInstallationAttribute>() == null))
            {
                Add(type, logNameMapper, sourceNameMapper);
            }
        }
    }
}