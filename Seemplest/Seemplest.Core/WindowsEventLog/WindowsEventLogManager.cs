using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This class provides oparation to manage event logs and log sources
    /// </summary>
    public static class WindowsEventLogManager
    {
        /// <summary>
        /// Installs Windows event log sources provided by the the argument
        /// </summary>
        /// <param name="data">Data to create performance counters</param>
        public static WindowsLogInstallationResult InstallEventLogSources(EventLogCreationData data)
        {
            var errors = new Dictionary<string, Exception>();
            var installedSources = new Dictionary<string, List<string>>();

            if (data == null) throw new ArgumentNullException("data");

            // --- Install sources one-by-one
            foreach (var creationData in data.SourceCreationData)
            {
                try
                {
                    // --- Check if source already exists
                    if (EventLog.SourceExists(creationData.Source))
                    {
                        // --- Check, if the appripriate log holds the source
                        var sourceLog = EventLog.LogNameFromSourceName(creationData.Source, ".");
                        if (sourceLog != creationData.LogName)
                        {
                            // --- Remove the source from its current log
                            EventLog.DeleteEventSource(creationData.Source);
                            EventLog.CreateEventSource(creationData);
                        }
                    }
                    else
                    {
                        // --- At this point the source can be created
                        EventLog.CreateEventSource(creationData);
                    }
                    
                    // --- Add source to results
                    List<string> sources;
                    if (!installedSources.TryGetValue(creationData.LogName, out sources))
                    {
                        installedSources.Add(creationData.LogName, new List<string> { creationData.Source });
                    }
                    else
                    {
                        sources.Add(creationData.Source);
                    }
                }
                catch (Exception ex)
                {
                    errors[creationData.Source] = ex;
                }
            }
            return new WindowsLogInstallationResult(installedSources, errors);
        }

        /// <summary>
        /// Removes Windows event log sources provided by the the argument
        /// </summary>
        /// <param name="data">Data to create performance counters</param>
        public static WindowsLogInstallationResult RemoveEventLogSources(EventLogCreationData data)
        {
            var errors = new Dictionary<string, Exception>();
            var installedSources = new Dictionary<string, List<string>>();

            if (data == null) throw new ArgumentNullException("data");

            // --- Install sources one-by-one
            foreach (var creationData in data.SourceCreationData)
            {
                // --- Check if source already exists
                if (!EventLog.SourceExists(creationData.Source)) continue;

                // --- Check, if the appropriate log holds the source
                var sourceLog = EventLog.LogNameFromSourceName(creationData.Source, ".");
                if (sourceLog != creationData.LogName) continue;

                // --- Remove the source from its current log
                EventLog.DeleteEventSource(creationData.Source);

                // --- Add source to results
                List<string> sources;
                if (!installedSources.TryGetValue(creationData.LogName, out sources))
                {
                    installedSources.Add(creationData.LogName, new List<string> {creationData.Source});
                }
                else
                {
                    sources.Add(creationData.Source);
                }
            }
            return new WindowsLogInstallationResult(installedSources, errors);
        }

        /// <summary>
        /// Stores the result of Windows event log source installation
        /// </summary>
        public class WindowsLogInstallationResult
        {
            public Dictionary<string, List<string>> AffectedSources { get; private set; }

            /// <summary>
            /// Errors during the installation
            /// </summary>
            public Dictionary<string, Exception> Errors { get; private set; }

            /// <summary>
            /// Creates new instance of results of the installation of Windows event log sources
            /// </summary>
            /// <param name="installedSources">Successfully installed categories list</param>
            /// <param name="errors">Errors during the installation</param>
            public WindowsLogInstallationResult(Dictionary<string, List<string>> installedSources, Dictionary<string, Exception> errors)
            {
                AffectedSources = installedSources;
                Errors = errors;
            }
        }
    }
}