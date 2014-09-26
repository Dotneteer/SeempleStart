using System;

namespace Seemplest.Core.Configuration
{
    public static class EnvironmentInfo
    {
        /// <summary>
        /// Initializes the static members of the provider
        /// </summary>
        static EnvironmentInfo()
        {
            Reset();
        }

        /// <summary>
        /// Sets the provider to the default one
        /// </summary>
        public static void Reset()
        {
            Provider = new DefaultEnvironmentInfoProvider();
        }

        /// <summary>
        /// Gets the current provider
        /// </summary>
        public static IEnvironmentInfoProvider Provider { get; private set; }

        /// <summary>
        /// Sets the current provider to the specified one.
        /// </summary>
        /// <param name="provider">Environment information provider instance</param>
        public static void Configure(IEnvironmentInfoProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            Provider = provider;
        }

        /// <summary>
        /// Gets the current UTC date and time
        /// </summary>
        /// <returns>Current UTC date and time</returns>
        public static DateTime GetCurrentDateTimeUtc()
        {
            return Provider.GetCurrentDateTimeUtc();
        }

        /// <summary>
        /// Gets this machine's name
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {
            return Provider.GetMachineName();
        }
    }
}