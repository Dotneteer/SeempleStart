using System;

namespace Seemplest.Core.Configuration
{
    /// <summary>
    /// This class defines the operations of an environment information provider.
    /// </summary>
    public interface IEnvironmentInfoProvider
    {
        /// <summary>
        /// Gets the current UTC date and time
        /// </summary>
        /// <returns>Current UTC date and time</returns>
        DateTime GetCurrentDateTimeUtc();

        /// <summary>
        /// Gets this machine's name
        /// </summary>
        /// <returns></returns>
        string GetMachineName();
    }
}