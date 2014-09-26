using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Seemplest.Core.DataAccess.DataServices;
using SeemplestBlocks.Core.AppConfig.DataAccess;
using SeemplestBlocks.Core.AppConfig.Exceptions;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This class uses SQL Server database to store and retrieve configuration values.
    /// </summary>
    public class MsSqlConfigurationHandler: IConfigurationHandler
    {
        private static volatile Dictionary<string, List<Tuple<string, string>>> s_Categories;

        private static readonly TimeSpan s_DefaultRetention = TimeSpan.FromMinutes(1);
        private static TimeSpan s_Retention;
        private static bool s_IsRefreshInProgress;
        private static readonly object s_Locker = new object();
        private static bool s_Hold;

        /// <summary>
        /// Initializes the static members of this class
        /// </summary>
        static MsSqlConfigurationHandler()
        {
            Reset(s_DefaultRetention);
        }

        /// <summary>
        /// Sets the cache expiration time, and clears the contents of the cache
        /// </summary>
        /// <param name="retentionTimeSpan">Cache retention time</param>
        public static void Reset(TimeSpan retentionTimeSpan)
        {
            lock (s_Locker)
            {
                s_Categories = null;
                s_IsRefreshInProgress = false;
                s_Retention = retentionTimeSpan;
                LastRefreshTime = DateTime.Now;
                LastRefreshException = null;
                s_Hold = false;
            }
        }

        /// <summary>
        /// Temporarily disables refreshing cache values
        /// </summary>
        public static void Hold()
        {
            s_Hold = true;
        }

        /// <summary>
        /// The last refresh time of the cache
        /// </summary>
        public static DateTime? LastRefreshTime { get; private set; }

        /// <summary>
        /// The exception raised during the last refresh
        /// </summary>
        public static Exception LastRefreshException { get; private set; }

        /// <summary>
        /// Reads the configuration value with the specified key
        /// </summary>
        /// <param name="category">Category of the configuration item</param>
        /// <param name="key">The key of the configuration item</param>
        /// <param name="value">Configuration value, provided, it is found</param>
        /// <returns>True, if the configuration value is found; otherwise, false</returns>
        public bool GetConfigurationValue(string category, string key, out string value)
        {
            EnsureConfigurationIsRefreshed();
            List<Tuple<string, string>> valueList;
            if (!s_Categories.TryGetValue(category, out valueList))
            {
                value = null;
                return false;
            }
            var valueInList = valueList
                .FirstOrDefault(v => String.Compare(v.Item1, key, StringComparison.OrdinalIgnoreCase) == 0);
            if (valueInList == null)
            {
                value = null;
                return false;
            }
            value = valueInList.Item2;
            return true;
        }

        /// <summary>
        /// Stores the specified configuration value
        /// </summary>
        /// <param name="category">Configuration item category</param>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Configuration value</param>
        public void SetConfigurationValue(string category, string key, string value)
        {
            using (var ctx = DataAccessFactory.CreateContext<IConfigurationDataOperations>())
            {
                var currentVersion = ctx.GetCurrentConfigurationVersion();
                if (currentVersion == null)
                {
                    throw new CurrentVersionNotFoundException();
                }
                var versionId = currentVersion.CurrentVersion;

                if (value == null)
                {
                    ctx.DeleteConfigurationValue(versionId, category, key);
                    return;
                }

                var valueInDb = ctx.GetConfigurationValueByKey(versionId, category, key);
                if (valueInDb == null)
                {
                    ctx.InsertConfigurationValue(new ConfigurationValueRecord
                    {
                        VersionId = versionId,
                        Category = category,
                        ConfigKey = key,
                        Value = value
                    });
                }
                else if (value != valueInDb.Value)
                {
                    valueInDb.Value = value;
                    ctx.UpdateConfigurationValue(valueInDb);
                }
            }
        }

        /// <summary>
        /// Checks whether the configuration value changes since the last check
        /// </summary>
        /// <param name="category">Configuration item category</param>
        /// <param name="key">Configuration key</param>
        /// <returns>True, if the configuration vale has changed; otherwise, false</returns>
        [ExcludeFromCodeCoverage]
        public bool ChangedSinceLastCheck(string category, string key)
        {
            return !s_Hold && DateTime.Now - LastRefreshTime > s_Retention;
        }

        /// <summary>
        /// Gets the current configuration version
        /// </summary>
        /// <returns>Configuration version key</returns>
        public int? GetCurrentConfigurationVersion()
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                var currentVersion = ctx.GetCurrentConfigurationVersion();
                return currentVersion == null ? (int?) null : currentVersion.CurrentVersion;
            }
        }

        /// <summary>
        /// Ensures that the configuration is refreshed
        /// </summary>
        private void EnsureConfigurationIsRefreshed()
        {
            if (s_Categories == null)
            {
                lock (s_Locker)
                {
                    if (s_Categories == null)
                    {
                        s_Categories = PopulateValues();
                    }
                }
            }

            if (!ChangedSinceLastCheck("", "")) return;

            lock (s_Locker)
            {
                if (s_IsRefreshInProgress) return;
                s_IsRefreshInProgress = true;
            }
            var task = new Task<Dictionary<string, List<Tuple<string, string>>>>(PopulateValues);
            task.ContinueWith(t =>
            {
                lock (s_Locker)
                {
                    LastRefreshException = t.Exception;
                    s_Categories = t.Result;
                    LastRefreshTime = DateTime.Now;
                    s_IsRefreshInProgress = false;
                }
            });
            task.Start();
        }

        /// <summary>
        /// Reads the coinfiguration values from the database
        /// </summary>
        /// <returns>
        /// The full configuration value set
        /// </returns>
        private static Dictionary<string, List<Tuple<string, string>>> PopulateValues()
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                var configRecords = ctx.GetAllConfigurationValues();
                return configRecords
                    .GroupBy(r => r.Category)
                    .ToDictionary(g => g.Key, g => g.Select(r => new Tuple<string, string>(r.ConfigKey, r.Value)).ToList());
            }
        }
    }
}