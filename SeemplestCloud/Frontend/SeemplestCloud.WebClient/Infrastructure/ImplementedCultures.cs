using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SeemplestCloud.WebClient.Infrastructure
{
    /// <summary>
    /// This class provides helper functions to describe the cultures supported in 
    /// this web site
    /// </summary>
    public static class ImplementedCultures
    {
        private static readonly List<CultureDescriptor> s_Cultures =
            new List<CultureDescriptor>
            {
                new CultureDescriptor("hu-HU", "Magyar", "Hungarian.png"),
                new CultureDescriptor("de-DE", "Deutsch", "German.png"),
                new CultureDescriptor("en-US", "English", "English.png")
            };

        /// <summary>
        /// Gets the list of culture codes
        /// </summary>
        /// <returns>List of culture codes</returns>
        public static IReadOnlyCollection<string> GetCultureCodes()
        {
            return new ReadOnlyCollection<string>(s_Cultures.Select(c => c.Code).ToList());
        }

        /// <summary>
        /// Gets the list of culture descriptions
        /// </summary>
        /// <returns>List of culture descriptions</returns>
        public static ReadOnlyCollection<CultureDescriptor> GetCultureDescriptors()
        {
            return new ReadOnlyCollection<CultureDescriptor>(s_Cultures);
        }

        /// <summary>
        /// Gets the name of the specified culture
        /// </summary>
        /// <param name="code">Culture code</param>
        /// <returns>Culture name to display</returns>
        public static string GetLanguageName(string code)
        {
            return s_Cultures.FirstOrDefault(c => c.Code == code).Name;
        }

        /// <summary>
        /// Gets the name of the specified culture's icon
        /// </summary>
        /// <param name="code">Culture code</param>
        /// <returns>Culture name to display</returns>
        public static string GetLanguageIcon(string code)
        {
            return s_Cultures.FirstOrDefault(c => c.Code == code).IconName;
        }

        /// <summary>
        /// This structure describes a culture
        /// </summary>
        public struct CultureDescriptor
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string IconName { get; set; }

            public CultureDescriptor(string code, string name, string iconName) : this()
            {
                Code = code;
                Name = name;
                IconName = iconName;
            }
        }
    }
}