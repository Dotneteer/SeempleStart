using System;
using System.Collections.Generic;
using System.Linq;
using Seemplest.Core.DependencyInjection;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This class provides localized resource management for backend services
    /// </summary>
    public static class LocalizedResourceManager
    {
        private static TimeSpan s_ExpirationSpan;
        private static readonly Dictionary<string, Tuple<DateTime, Dictionary<string, string>>> s_Cache =
            new Dictionary<string, Tuple<DateTime, Dictionary<string, string>>>();
        private static IConfigurationService s_Service;

        /// <summary>
        /// Sets the default parameters of this service
        /// </summary>
        static LocalizedResourceManager()
        {
            Reset(TimeSpan.FromMinutes(15));
        }

        /// <summary>
        /// Sets the expiration time of cached items, clears the content of resource cache
        /// </summary>
        /// <param name="expirationSpan">Expiration time span</param>
        public static void Reset(TimeSpan expirationSpan)
        {
            s_ExpirationSpan = expirationSpan;
            s_Cache.Clear();
            s_Service = ServiceManager.GetService<IConfigurationService>();
        }

        /// <summary>
        /// Gets the resource value by the specified local, category, and key
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="category">Resource category</param>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Resource value, if found; otherwise, null</returns>
        /// <remarks>
        /// This operation tries to get the resource value in these steps:
        /// 1) The specified locale (pl. "en-us"); if not found, then
        /// 2) Only the language part is used (pl. "en"); if still not found, then
        /// 3) default locale is used ("def"); otherwise
        /// 4) "null" is returned
        /// </remarks>
        public static string GetResourceByLocale(string locale, string category, string resourceKey)
        {
            var value = GetExplicitResource(locale, category, resourceKey);
            if (value != null) return value;

            if (locale.Length > 2)
            {
                var language = locale.Substring(0, 2);
                value = GetExplicitResource(language, category, resourceKey);
                if (value != null) return value;
            }

            value = GetExplicitResource("def", category, resourceKey);
            return value;
        }

        /// <summary>
        /// Gets the specified explicit resoure
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="category">Resource category</param>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Resource value, if found; otherwise, null</returns>
        private static string GetExplicitResource(string locale, string category, string resourceKey)
        {
            var key = GetCategoryKey(locale, category);
            Tuple<DateTime, Dictionary<string, string>> resourcesInCategory;
            if (s_Cache.TryGetValue(key, out resourcesInCategory))
            {
                if (resourcesInCategory.Item1 < DateTime.Now) resourcesInCategory = null;
            }

            if (resourcesInCategory == null)
            {
                var resources = s_Service
                    .GetLocalizedResourcesByCategory(locale, category)
                    .ToDictionary(i => i.ResourceKey, i => i.Value);
                s_Cache[key] = resourcesInCategory = new Tuple<DateTime, Dictionary<string, string>>(
                    DateTime.Now + s_ExpirationSpan,
                    resources);
            }

            var resourceDict = resourcesInCategory.Item2;
            string resourceValue;
            return resourceDict.TryGetValue(resourceKey, out resourceValue)
                ? resourceValue : null;
        }

        /// <summary>
        /// Checks whether the specified resource is in the cache
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="category">Resource category</param>
        /// <returns>True, if the category is in the cache; otherwise, false
        /// </returns>
        public static bool IsCached(string locale, string category)
        {
            var key = GetCategoryKey(locale, category);
            Tuple<DateTime, Dictionary<string, string>> resourcesInCategory;
            if (s_Cache.TryGetValue(key, out resourcesInCategory))
            {
                // --- Ha bent volt a tárban, de lejárt, nincs bent
                if (resourcesInCategory.Item1 < DateTime.Now) resourcesInCategory = null;
            }
            return resourcesInCategory != null;
        }

        /// <summary>
        /// Gets the key for the resource
        /// </summary>
        private static string GetCategoryKey(string locale, string category)
        {
            return String.Format("{0}:{1}", locale, category);
        }

    }
}