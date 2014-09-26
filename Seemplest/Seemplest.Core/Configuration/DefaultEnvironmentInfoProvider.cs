using System;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class implements the default information provider
    /// </summary>
    public class DefaultEnvironmentInfoProvider : IEnvironmentInfoProvider
    {
        /// <summary>
        /// Gets the current UTC date and time
        /// </summary>
        /// <returns>Current UTC date and time</returns>
        public DateTime GetCurrentDateTimeUtc()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Gets this machine's name
        /// </summary>
        /// <returns></returns>
        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}