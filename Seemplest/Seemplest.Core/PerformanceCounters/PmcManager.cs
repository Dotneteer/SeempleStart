using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Seemplest.Core.Configuration;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class provides operations to manage performance counters
    /// </summary>
    public static class PmcManager
    {
        private static readonly Dictionary<CounterKey, PerformanceCounterHandle> s_CounterCache = 
            new Dictionary<CounterKey, PerformanceCounterHandle>();

        /// <summary>
        /// Installs performance counters provided by the the argument
        /// </summary>
        /// <param name="data">Data to create performance counters</param>
        public static PmcInstallationResult InstallPerformanceCounters(PmcCreationData data)
        {
            var errors = new Dictionary<string, Exception>();
            var categories = new List<string>();

            if (data == null) throw new ArgumentNullException("data");

            // --- Install categories one-by-one
            foreach (var category in data.Categories.Values.Where(category => !category.IsPredefined))
            {
                var counterData = new CounterCreationDataCollection();
                foreach (var counter in data.GetCounters(category.Name)
                    .Values.Where(counter => !counter.IsPredefined))
                {
                    counterData.Add(new CounterCreationData(counter.Name, counter.Help, counter.Type));
                }
                var categoryName = AppConfigurationManager.ProviderSettings.InstancePrefix + category.Name;
                try
                {
                    // --- Delete the whole category
                    if (PerformanceCounterCategory.Exists(categoryName))
                    {
                        PerformanceCounterCategory.Delete(categoryName);
                    }

                    // --- Add the whole category again
                    PerformanceCounterCategory.Create(
                        categoryName,
                        category.Help,
                        category.Type, 
                        counterData);
                    categories.Add(category.Name);
                }
                catch (Exception ex)
                {
                    // --- Administer errors
                    errors[category.Name] = ex;
                }
            }
            return new PmcInstallationResult(categories, errors);
        }

        /// <summary>
        /// Removes the performance counter data specified in the arguments
        /// </summary>
        /// <param name="data">Data to remove performance counters</param>
        /// <returns>Installation result</returns>
        public static PmcInstallationResult RemovePerformanceCounters(PmcCreationData data)
        {
            var errors = new Dictionary<string, Exception>();
            var categories = new List<string>();

            if (data == null) throw new ArgumentNullException("data");

            // --- Remove categories one-by-one
            foreach (var category in data.Categories.Values.Where(category => !category.IsPredefined))
            {
                var categoryName = AppConfigurationManager.ProviderSettings.InstancePrefix + category.Name;
                try
                {
                    // --- Delete the whole category
                    if (PerformanceCounterCategory.Exists(categoryName))
                    {
                        PerformanceCounterCategory.Delete(categoryName);
                    }
                    categories.Add(categoryName);
                }
                catch (Exception ex)
                {
                    // --- Administer errors
                    errors[categoryName] = ex;
                }
            }
            return new PmcInstallationResult(categories, errors);
        }

        /// <summary>
        /// Gets a performance counter handle for the specified type and instance.
        /// </summary>
        /// <param name="counterType">Counter definition type</param>
        /// <param name="instanceName">Name of the performance counter instance</param>
        /// <returns>Counter instance</returns>
        public static PerformanceCounterHandle GetCounter(Type counterType, string instanceName = null)
        {
            // --- Check arguments
            if (counterType == null) throw new ArgumentNullException("counterType");
            if (!counterType.IsSubclassOf(typeof(PmcDefinitionBase)))
            {
                throw new InvalidOperationException(
                    String.Format("{0} type is not derived from PmcDefinitionBase.", counterType));
            }

            // --- Try to obtain the instance from the cache
            var key = new CounterKey(counterType, instanceName);
            PerformanceCounterHandle handle;
            if (!s_CounterCache.TryGetValue(key, out handle))
            {
                handle = new PerformanceCounterHandle(counterType, instanceName);
                s_CounterCache[key] = handle;
            }
            return handle;
        }

        /// <summary>
        /// Gets a performance counter handle for the specified type and instance.
        /// </summary>
        /// <typeparam name="TCounter">Counter definition type</typeparam>
        /// <param name="instanceName">Name of the performance counter instance</param>
        /// <returns>Counter instance</returns>
        public static PerformanceCounterHandle GetCounter<TCounter>(string instanceName = null)
        {
            return GetCounter(typeof (TCounter), instanceName);
        }

        /// <summary>
        /// Stores the result of performance counter installation
        /// </summary>
        public class PmcInstallationResult
        {
            /// <summary>
            /// Successfully installed categories list
            /// </summary>
            public List<string> InstalledCategories { get; private set; }

            /// <summary>
            /// Errors during the installation
            /// </summary>
            public Dictionary<string, Exception> Errors { get; private set; }

            /// <summary>
            /// Creates new instance of results of the installation of the performance counters
            /// </summary>
            /// <param name="installedCategories">Successfully installed categories list</param>
            /// <param name="errors">Errors during the installation</param>
            public PmcInstallationResult(List<string> installedCategories, Dictionary<string, Exception> errors)
            {
                InstalledCategories = installedCategories;
                Errors = errors;
            }
        }
        
        /// <summary>
        /// Represents a counter key.
        /// </summary>
        class CounterKey: Tuple<Type, string>
        {
            public CounterKey(Type type, string instanceName) : base(type, instanceName)
            {
            }
        }
    }
}