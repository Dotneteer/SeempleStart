using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This class defines extension methods for helper types
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Converts the specified object's properties and values into a RouteValueDictionary
        /// instance.
        /// </summary>
        /// <param name="data">The object to convert</param>
        /// <returns>The dictionary with properties and values</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RouteValueDictionary ToDictionary(this object data)
        {
            return HtmlHelper.AnonymousObjectToHtmlAttributes(data);
        }

        /// <summary>
        /// Merges two dictionaries holding HTML attribute definitions.
        /// </summary>
        /// <remarks>
        /// If key exists, it overrides it with new value.
        /// In case it's a class, it will add new values to existing.
        /// </remarks>
        public static void MergeHtmlAttributes(this IDictionary<string, object> source, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                throw new ArgumentNullException("htmlAttributes");
            }

            foreach (var item in htmlAttributes)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                    if (item.Key.ToLower() == "class")
                    {
                        source[item.Key] = source[item.Key] + " " + item.Value;
                    }
                    else
                    {
                        source[item.Key] = item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Converts the specified object to a dictionary of HTML attributes
        /// </summary>
        /// <param name="data">Object to convert to HTML attributes</param>
        /// <returns></returns>
        public static IDictionary<string, object> ObjectToHtmlAttributesDictionary(this object data)
        {
            if (data == null)
            {
                return new Dictionary<string, object>();
            }
            var dictionary = data as IDictionary<string, object> 
                ?? data.ToDictionary();
            return dictionary;
        }

        /// <summary>
        /// Formats the specified HTML attributes according to formatting rules
        /// </summary>
        /// <param name="htmlAttributes">Set of attributes to format</param>
        /// <returns></returns>
        public static IDictionary<string, object> FormatHtmlAttributes(this IDictionary<string, object> htmlAttributes)
        {
            // TODO: refactor this method to support camel casing and PascalCasing
            return htmlAttributes == null 
                ? new Dictionary<string, object>() 
                : htmlAttributes.Keys.ToDictionary(key => key.Replace('_', '-'), key => htmlAttributes[key]);
        }

        /// <summary>
        /// Checks and adjust column width
        /// </summary>
        /// <param name="width">Width value</param>
        /// <returns>Adjusted width value</returns>
        public static int ColumnWidth(this int width)
        {
            return width < 1 
                ? 1 
                : (width > 12 ? 12 : width);
        }
    }
}