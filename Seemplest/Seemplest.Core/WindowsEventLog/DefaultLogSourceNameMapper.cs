using Seemplest.Core.Common;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// Maps names to themselves with an instance prefix
    /// </summary>
    public class DefaultLogSourceNameMapper : INameMapper
    {
        /// <summary>
        /// Gets the name of the instance that should prepend the log source name.
        /// </summary>
        public string InstancePrefix { get; private set; }

        /// <summary>
        /// Initializes this instance with the specified instance name
        /// </summary>
        /// <param name="instanceName">Instance name</param>
        public DefaultLogSourceNameMapper(string instanceName = null)
        {
            InstancePrefix = instanceName 
                ?? AppConfigurationManager.ProviderSettings.InstancePrefix;
        }

        /// <summary>
        /// Prepends <see cref="InstancePrefix"/> to the specified name.
        /// </summary>
        /// <param name="name">Source name</param>
        /// <returns>Mapped name</returns>
        public string Map(string name)
        {
            return InstancePrefix + name;
        }
    }
}