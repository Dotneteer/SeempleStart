using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Seemplest.Core.PerformanceCounters
{
    /// <summary>
    /// This class provides performance data to create performance counters
    /// </summary>
    public class PmcCreationData
    {
        private readonly Dictionary<string, PmcCategoryDefinitionBase> _pmcCategories = 
            new Dictionary<string, PmcCategoryDefinitionBase>();
        private readonly Dictionary<string, Dictionary<string, PmcDefinitionBase>> _counters = 
            new Dictionary<string, Dictionary<string, PmcDefinitionBase>>();

        /// <summary>
        /// Clears the collection of counters
        /// </summary>
        public void Clear()
        {
            _pmcCategories.Clear();
            _counters.Clear();
        }

        /// <summary>
        /// Adds the specified category or counter type to the creation data
        /// </summary>
        /// <param name="counterType">Category or counter type to add</param>
        public void Add(Type counterType)
        {
            var baseType = counterType.BaseType;
            if (baseType == typeof (PmcCategoryDefinitionBase))
            {
                // --- Add the specified cateogyr
                AddCategory(counterType);
            }
            else if (baseType != null && baseType.IsConstructedGenericType && 
                baseType.GetGenericTypeDefinition() == typeof(PmcDefinitionBase<>))
            {
                // --- Add the specified conuter, first add its category data
                var categoryType = baseType.GetGenericArguments()[0];
                var categoryName = AddCategory(categoryType).Name;

                // --- Obtain counters for the category
                Dictionary<string, PmcDefinitionBase> counterData;
                if (!_counters.TryGetValue(categoryName, out counterData))
                {
                    counterData = new Dictionary<string, PmcDefinitionBase>();
                    _counters.Add(categoryName, counterData);
                }

                // --- Check counter by its name
                var instance = Activator.CreateInstance(counterType) as PmcDefinitionBase;
                Debug.Assert(instance != null, "instance != null");
                if (counterData.ContainsKey(instance.Name))
                {
                    throw new InvalidOperationException(
                    string.Format("Performnace counter with name {0} already exists.", instance.Name));
                }
                counterData.Add(instance.Name, instance);
            }
            else
            {
                throw new InvalidOperationException(
                string.Format("Add must be called with a PmcDefinitionBase or PmcCategoryDefinitionBase derived type. "
                    + "{0} is not an appropriate type.", counterType));
            }
        }
        
        /// <summary>
        /// Merges all performance counter types from the specified assembly with the 
        /// creation data.
        /// </summary>
        /// <param name="assembly">Assembly to scan for perfromance counter types</param>
        public void MergeCountersFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly
                .GetTypes()
                .Where(type => type.BaseType != null && type.BaseType.IsConstructedGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof (PmcDefinitionBase<>)))
            {
                Add(type);
            }
        }

        /// <summary>
        /// Gets the performance categories
        /// </summary>
        public IReadOnlyDictionary<string, PmcCategoryDefinitionBase> Categories
        {
            get { return new ReadOnlyDictionary<string, PmcCategoryDefinitionBase>(_pmcCategories); }
        }

        /// <summary>
        /// Gets the performance counters defined within the specified category
        /// </summary>
        /// <param name="category">Category to obtain the counters for</param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, PmcDefinitionBase> GetCounters(string category)
        {
            return new ReadOnlyDictionary<string, PmcDefinitionBase>(_counters[category]);
        }

        /// <summary>
        /// Adds a category type to the creation data.
        /// </summary>
        /// <param name="categoryType">Category type</param>
        /// <returns>Category instance</returns>
        private PmcCategoryDefinitionBase AddCategory(Type categoryType)
        {
            var instance = Activator.CreateInstance(categoryType) as PmcCategoryDefinitionBase;
            PmcCategoryDefinitionBase storedInstance;
            // ReSharper disable PossibleNullReferenceException
            if (_pmcCategories.TryGetValue(instance.Name, out storedInstance))
            // ReSharper restore PossibleNullReferenceException
            {
                if (storedInstance.GetType() != categoryType)
                {
                    throw new InvalidOperationException(
                        string.Format("{0} and {1} performance category types both declare '{2}' as category name",
                                      categoryType, storedInstance, instance.Name));
                }
            }
            else
            {
                _pmcCategories.Add(instance.Name, instance);
            }
            return instance;
        }
    }
}