using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SeemplestBlocks.Dto.Internationalization;

namespace SeemplestBlocks.Core.Internationalization
{
    /// <summary>
    /// This controller allows retrieving server side resource files as 
    /// dictionaries
    /// </summary>
    public abstract class ResourceControllerBase : LanguageAwareApiControllerBase
    {
        /// <summary>
        /// Gets the resource values defined by the specified type
        /// </summary>
        /// <param name="resourceType">Resource type</param>
        /// <returns>List of parameters</returns>
        protected List<ResourceStringDto> GetResources(Type resourceType)
        {
            var result = resourceType.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(t => t.PropertyType == typeof(string))
                .Select(t => new ResourceStringDto
                {
                    Code = t.Name,
                    Value = t.GetValue(null).ToString()
                }).ToList();
            return result;
        }
    }
}
