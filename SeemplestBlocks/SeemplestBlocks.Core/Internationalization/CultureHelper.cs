using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SeemplestBlocks.Core.Internationalization
{
    /// <summary>
    /// This class provides useful functions for internationalization tasks
    /// </summary>
    public static class CultureHelper
    {
        // --- Valid cultures
        private static readonly List<string> s_ValidCultures = new List<string>
        {
            "af", "af-ZA", "sq", "sq-AL", "gsw-FR", "am-ET", "ar", "ar-DZ", "ar-BH", 
            "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", "ar-MA", "ar-OM", 
            "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-AE", "ar-YE", "hy", "hy-AM", 
            "as-IN", "az", "az-Cyrl-AZ", "az-Latn-AZ", "ba-RU", "eu", "eu-ES", 
            "be", "be-BY", "bn-BD", "bn-IN", "bs-Cyrl-BA", "bs-Latn-BA", "br-FR", 
            "bg", "bg-BG", "ca", "ca-ES", "zh-HK", "zh-MO", "zh-CN", "zh-Hans", 
            "zh-SG", "zh-TW", "zh-Hant", "co-FR", "hr", "hr-HR", "hr-BA", "cs", 
            "cs-CZ", "da", "da-DK", "prs-AF", "div", "div-MV", "nl", "nl-BE", 
            "nl-NL", "en", "en-AU", "en-BZ", "en-CA", "en-029", "en-IN", "en-IE", 
            "en-JM", "en-MY", "en-NZ", "en-PH", "en-SG", "en-ZA", "en-TT", "en-GB", 
            "en-US", "en-ZW", "et", "et-EE", "fo", "fo-FO", "fil-PH", "fi", "fi-FI", 
            "fr", "fr-BE", "fr-CA", "fr-FR", "fr-LU", "fr-MC", "fr-CH", "fy-NL", "gl", 
            "gl-ES", "ka", "ka-GE", "de", "de-AT", "de-DE", "de-LI", "de-LU", "de-CH", 
            "el", "el-GR", "kl-GL", "gu", "gu-IN", "ha-Latn-NG", "he", "he-IL", "hi", 
            "hi-IN", "hu", "hu-HU", "is", "is-IS", "ig-NG", "id", "id-ID", "iu-Latn-CA", 
            "iu-Cans-CA", "ga-IE", "xh-ZA", "zu-ZA", "it", "it-IT", "it-CH", "ja", 
            "ja-JP", "kn", "kn-IN", "kk", "kk-KZ", "km-KH", "qut-GT", "rw-RW", "sw", 
            "sw-KE", "kok", "kok-IN", "ko", "ko-KR", "ky", "ky-KG", "lo-LA", "lv", 
            "lv-LV", "lt", "lt-LT", "wee-DE", "lb-LU", "mk", "mk-MK", "ms", "ms-BN", 
            "ms-MY", "ml-IN", "mt-MT", "mi-NZ", "arn-CL", "mr", "mr-IN", "moh-CA", 
            "mn", "mn-MN", "mn-Mong-CN", "ne-NP", "no", "nb-NO", "nn-NO", "oc-FR", 
            "or-IN", "ps-AF", "fa", "fa-IR", "pl", "pl-PL", "pt", "pt-BR", "pt-PT", 
            "pa", "pa-IN", "quz-BO", "quz-EC", "quz-PE", "ro", "ro-RO", "rm-CH", 
            "ru", "ru-RU", "smn-FI", "smj-NO", "smj-SE", "se-FI", "se-NO", "se-SE", 
            "sms-FI", "sma-NO", "sma-SE", "sa", "sa-IN", "sr", "sr-Cyrl-BA", "sr-Cyrl-SP", 
            "sr-Latn-BA", "sr-Latn-SP", "nso-ZA", "tn-ZA", "si-LK", "sk", "sk-SK", 
            "sl", "sl-SI", "es", "es-AR", "es-BO", "es-CL", "es-CO", "es-CR", "es-DO", 
            "es-EC", "es-SV", "es-GT", "es-HN", "es-MX", "es-NI", "es-PA", "es-PY", 
            "es-PE", "es-PR", "es-ES", "es-US", "es-UY", "es-VE", "sv", "sv-FI", 
            "sv-SE", "syr", "syr-SY", "tg-Cyrl-TJ", "tzm-Latn-DZ", "ta", "ta-IN", "tt", 
            "tt-RU", "te", "te-IN", "th", "th-TH", "bo-CN", "tr", "tr-TR", "tk-TM", 
            "ug-CN", "uk", "uk-UA", "wen-DE", "ur", "ur-PK", "uz", "uz-Cyrl-UZ", 
            "uz-Latn-UZ", "vi", "vi-VN", "cy-GB", "wo-SN", "sah-RU", "ii-CN", "yo-NG"
        };

        // --- This list holds the currently implemented cultures
        private static readonly List<string> s_Cultures = new List<string>();

        /// <summary>
        /// Use this method to set the cultures implemented by your application
        /// </summary>
        /// <param name="cultures">Implemented cultures to set</param>
        public static void SetImplementedCultures(IEnumerable<string> cultures)
        {
            s_Cultures.Clear();
            s_Cultures.AddRange(cultures.Where(culture => 
                s_ValidCultures.Contains(culture, StringComparer.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets the list of implemented cultures
        /// </summary>
        /// <returns>List of implemented cultures</returns>
        public static IReadOnlyList<string> GetImplementedCultures()
        {
            return new ReadOnlyCollection<string>(s_Cultures); 
        }

        /// <summary>
        /// Returns true if the language is a right-to-left language. Otherwise, false.
        /// </summary>      
        public static bool IsRighToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

        }

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, 
        /// it returns the default culture "en-US"
        /// </summary>
        /// <param name="name">Culture's name (e.g. en-US)</param>
        public static string GetImplementedCulture(string name)
        {
            // --- Make sure it's not null
            if (string.IsNullOrEmpty(name))
            {
                return GetDefaultCulture();
            }

            // --- Make sure it is a valid culture first
            if (!s_ValidCultures.Any(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return GetDefaultCulture();
            }

            // --- If it is implemented, accept it
            if (s_Cultures.Any(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return name;
            }

            // --- Find a close match. For example, if you have "en-US" defined and 
            // --- the user requests "en-GB", the function will return closes match that 
            // --- is "en-US" because at least the language is the same (ie English)  
            var n = GetNeutralCulture(name);
            foreach (var c in s_Cultures.Where(c => c.StartsWith(n)))
            {
                return c;
            }

            // --- This culture is not implemented
            return GetDefaultCulture();
        }

        /// <summary>
        /// Returns default culture name which is the first name decalared (e.g. en-US)
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultCulture()
        {
            return s_Cultures[0]; // return Default culture

        }

        /// <summary>
        /// Gets the current culture
        /// </summary>
        /// <returns>Current culture name</returns>
        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// Gets the current neutral culture
        /// </summary>
        /// <returns>Current neutral culture name</returns>
        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }

        /// <summary>
        /// Creates a neutral name from culture name
        /// </summary>
        /// <param name="name">Full culture name</param>
        /// <returns>Neutral culture name</returns>
        public static string GetNeutralCulture(string name)
        {
            return !name.Contains("-") 
                ? name 
                : name.Split('-')[0];
        }

        /// <summary>
        /// Gets the English name of the specified implemented culture
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetImplementedCultureDisplayName(string name)
        {
            var baseName = CultureInfo.CreateSpecificCulture(name).EnglishName;
            return baseName.Substring(0, baseName.IndexOf('(')-1).Trim();
        }
    }
}