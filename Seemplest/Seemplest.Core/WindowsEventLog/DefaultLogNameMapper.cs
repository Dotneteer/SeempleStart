using Seemplest.Core.Common;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Maps names to themselves
    /// </summary>
    public class DefaultLogNameMapper : INameMapper
    {
        /// <summary>
        /// Gets the name of the default log
        /// </summary>
        public string DefaultName { get; private set; }

        /// <summary>
        /// Initializes this class with the specified default log name
        /// </summary>
        /// <param name="defaultName">Name of the default log</param>
        public DefaultLogNameMapper(string defaultName = "Application")
        {
            DefaultName = defaultName;
        }

        /// <summary>
        /// Maps the specified name to itself.
        /// </summary>
        /// <param name="name">Source name</param>
        /// <returns>Mapped name</returns>
        public string Map(string name)
        {
            return name == WindowsEventLogger.DEFAULT_APP_LOG ? DefaultName : name;
        }
    }
}