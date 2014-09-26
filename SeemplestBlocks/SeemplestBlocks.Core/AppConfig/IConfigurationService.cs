using System.Collections.Generic;
using Evolution.BuildingBlocks.Dto.Configuration;
using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This interface defines the operations of the configuration service.
    /// </summary>
    public interface IConfigurationService: IServiceObject
    {
        /// <summary>
        /// Gets all locales
        /// </summary>
        /// <returns>List of locales</returns>
        IList<LocaleDto> GetAllLocales();

        /// <summary>
        /// Checks the uniqoeness of a locale code
        /// </summary>
        /// <param name="code">Code of the locale</param>
        /// <returns>True, if the code is unique; otherwise, false</returns>
        bool IsLocaleCodeUnique(string code);

        /// <summary>
        /// Checks the uniqueness of the locale name
        /// </summary>
        /// <param name="name">Name of the locale</param>
        /// <returns>True, if the name is unique; otherwise, false</returns>
        bool IsLocaleDisplayNameUnique(string name);

        /// <summary>
        /// Adds a new locale to the existing ones
        /// </summary>
        /// <param name="locale">Locale information</param>
        void AddLocale(LocaleDto locale);

        /// <summary>
        /// Modifies an existing locale
        /// </summary>
        /// <param name="locale">Locale information</param>
        void ModifyLocale(LocaleDto locale);

        /// <summary>
        /// Removes the specified locale
        /// </summary>
        /// <param name="code">Code of the locale</param>
        void RemoveLocale(string code);

        /// <summary>
        /// Gets all resource categories
        /// </summary>
        /// <returns>List of resource categories</returns>
        List<string> GetLocalizedResourceCategories();
            
        /// <summary>
        /// Gets the resources in the specified category
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="category">Resource category</param>
        /// <returns>
        /// Resources in the specified category
        /// </returns>
        List<LocalizedResourceDto> GetLocalizedResourcesByCategory(string locale, string category);

        /// <summary>
        /// Clones resources of a source locale into a destination locale
        /// </summary>
        /// <param name="request">Clone request object</param>
        /// <remarks>
        /// The request can define the default resource value. If this is null, the source value
        /// is copied, otherwise, this value. Setting OverrideExistingResource to true will overwrite
        /// any resource values already set.
        /// </remarks>
        void CloneLocalizedResources(CloneLocalizedResourcesDto request);

        /// <summary>
        /// Gets the items of the specified list.
        /// </summary>
        /// <param name="listId">List identifier</param>
        /// <param name="locale">Locale code of list items</param>
        /// <returns>Items belonging to the specified list</returns>
        List<ListItemDefinitionDto> GetListItems(string listId, string locale = "def");
    }
}