using System.Collections.Generic;
using System.Linq;
using Evolution.BuildingBlocks.Dto.Configuration;
using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DataAccess.Exceptions;
using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Core.AppConfig.DataAccess;
using SeemplestBlocks.Core.AppConfig.Exceptions;
using SeemplestBlocks.Core.Diagnostics;

namespace SeemplestBlocks.Core.AppConfig
{
    /// <summary>
    /// This class implements the operations of the configuration service.
    /// </summary>
    [CommonDiagnosticsAspects]
    public class ConfigurationService : ServiceObjectBase, IConfigurationService
    {
        /// <summary>
        /// Resource category that stores list items
        /// </summary>
        public const string LIST_CATEGORY_NAME = "ListItems";

        /// <summary>
        /// Format of list item names
        /// </summary>
        public const string LIST_ITEM_NAME_FORMAT = "{0}.{1}.Name";

        /// <summary>
        /// Format of list item descriptions
        /// </summary>
        public const string LIST_ITEM_DESCRIPTION_FORMAT = "{0}.{1}.Description";

        /// <summary>
        /// Gets all locales
        /// </summary>
        /// <returns>List of locales</returns>
        public IList<LocaleDto> GetAllLocales()
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                return ctx.FetchAllLocales()
                    .Select(r => new LocaleDto
                    {
                        Code = r.Code,
                        DisplayName = r.DisplayName
                    }).ToList();
            }
        }

        /// <summary>
        /// Checks the uniqoeness of a locale code
        /// </summary>
        /// <param name="code">Code of the locale</param>
        /// <returns>True, if the code is unique; otherwise, false</returns>
        public bool IsLocaleCodeUnique(string code)
        {
            Verify.NotNullOrEmpty(code, "code");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                return ctx.GetByCode(code) == null;
            }
        }

        /// <summary>
        /// Checks the uniqueness of the locale name
        /// </summary>
        /// <param name="name">Name of the locale</param>
        /// <returns>True, if the name is unique; otherwise, false</returns>
        public bool IsLocaleDisplayNameUnique(string name)
        {
            Verify.NotNullOrEmpty(name, "name");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                return ctx.GetByName(name) == null;
            }
        }

        /// <summary>
        /// Adds a new locale to the existing ones
        /// </summary>
        /// <param name="locale">Locale information</param>
        public void AddLocale(LocaleDto locale)
        {
            VerifyLocaleDto(locale);

            using (var ctx = DataAccessFactory.CreateContext<IConfigurationDataOperations>())
            {
                try
                {
                    ctx.InsertLocale(new LocaleRecord
                    {
                        Code = locale.Code,
                        DisplayName = locale.DisplayName
                    });
                }
                catch (PrimaryKeyViolationException)
                {
                    throw new DuplicatedLocaleCodeException(locale.Code);
                }
                catch (UniqueKeyViolationException)
                {
                    throw new DuplicatedLocaleDisplayNameException(locale.DisplayName);
                }
            }
        }

        /// <summary>
        /// Modifies an existing locale
        /// </summary>
        /// <param name="locale">Locale information</param>
        public void ModifyLocale(LocaleDto locale)
        {
            VerifyLocaleDto(locale);

            using (var ctx = DataAccessFactory.CreateContext<IConfigurationDataOperations>())
            {
                var localeInDb = ctx.GetByCode(locale.Code);
                if (localeInDb == null)
                {
                    throw new LocaleNotFoundException(locale.Code);
                }
                localeInDb.DisplayName = locale.DisplayName;
                try
                {
                    ctx.UpdateLocale(localeInDb);
                }
                catch (UniqueKeyViolationException)
                {
                    throw new DuplicatedLocaleDisplayNameException(locale.DisplayName);
                }
            }
        }

        /// <summary>
        /// Removes the specified locale
        /// </summary>
        /// <param name="code">Code of the locale</param>
        public void RemoveLocale(string code)
        {
            Verify.NotNullOrEmpty(code, "code");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<IConfigurationDataOperations>())
            {
                var localeInDb = ctx.GetByCode(code);
                if (localeInDb == null)
                {
                    throw new LocaleNotFoundException(code);
                }

                ctx.BeginTransaction();

                ctx.DeleteResourcesOfLocale(code);
                ctx.DeleteLocale(code);

                ctx.Complete();
            }
        }

        /// <summary>
        /// Gets all resource categories
        /// </summary>
        /// <returns>List of resource categories</returns>
        public List<string> GetLocalizedResourceCategories()
        {
            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                return ctx.FetchAllCategories()
                    .Select(r => r.Category).ToList();
            }
        }

        /// <summary>
        /// Gets the resources in the specified category
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="category">Resource category</param>
        /// <returns>
        /// Resources in the specified category
        /// </returns>
        public List<LocalizedResourceDto> GetLocalizedResourcesByCategory(string locale, string category)
        {
            Verify.NotNull(locale, "locale");
            Verify.NotNull(category, "category");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                var records = ctx.GetLocalizedResourcesByCategory(locale, category);
                return records.Select(r => new LocalizedResourceDto
                {
                    Locale = r.Locale,
                    Category = r.Category,
                    ResourceKey = r.ResourceKey,
                    Value = r.Value
                }).ToList();
            }
        }

        /// <summary>
        /// Clones resources of a source locale into a destination locale
        /// </summary>
        /// <param name="request">Clone request object</param>
        /// <remarks>
        /// The request can define the default resource value. If this is null, the source value
        /// is copied, otherwise, this value. Setting OverrideExistingResource to true will overwrite
        /// any resource values already set.
        /// </remarks>
        public void CloneLocalizedResources(CloneLocalizedResourcesDto request)
        {
            Verify.NotNull(request, "request");
            Verify.RaiseWhenFailed();

            Verify.NotNullOrEmpty(request.BaseCode, "request.BaseCode");
            Verify.NotNullOrEmpty(request.TargetCode, "request.TargetCode");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateContext<IConfigurationDataOperations>())
            {
                if (ctx.GetByCode(request.BaseCode) == null)
                {
                    throw new LocaleNotFoundException(request.BaseCode);
                }
                if (ctx.GetByCode(request.TargetCode) == null)
                {
                    throw new LocaleNotFoundException(request.TargetCode);
                }

                ctx.BeginTransaction();

                if (request.OverrideExistingResources)
                {
                    ctx.DeleteExistingSourceResourcesFromTarget(request.BaseCode, request.TargetCode);
                }
                if (request.DefaultResourceValue != null)
                {
                    ctx.CloneResourcesWithDefault(request.BaseCode, request.TargetCode, request.DefaultResourceValue);
                }
                else
                {
                    ctx.CloneResources(request.BaseCode, request.TargetCode);
                }
                ctx.Complete();
            }
        }

        /// <summary>
        /// Gets the items of the specified list.
        /// </summary>
        /// <param name="listId">List identifier</param>
        /// <param name="locale">Locale code of list items</param>
        /// <returns>Items belonging to the specified list</returns>
        public List<ListItemDefinitionDto> GetListItems(string listId, string locale = "def")
        {
            Verify.NotNullOrEmpty(listId, "listId");
            Verify.NotNullOrEmpty(locale, "locale");
            Verify.RaiseWhenFailed();

            using (var ctx = DataAccessFactory.CreateReadOnlyContext<IConfigurationDataOperations>())
            {
                // --- Van ilyen lista?
                var listInfo = ctx.GetListDefinitionById(listId);
                if (listInfo == null)
                {
                    throw new ListDefinitionNotFoundException(listId);
                }

                return ctx.GetListItems(listId)
                    .Select(it => new ListItemDefinitionDto
                    {
                        Id = it.ItemId,
                        IsSystemItem = it.IsSystemItem,
                        DisplayName = GetListItemName(locale, listId, it.ItemId),
                        Description = GetListItemDescription(locale, listId, it.ItemId)
                    })
                    .ToList();
            }
        }

        #region Helper methods 

        /// <summary>
        /// Gets the name of the list item
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="listId">List identifier</param>
        /// <param name="itemId">List item identifier</param>
        /// <returns>Name of list item</returns>
        private static string GetListItemName(string locale, string listId, string itemId)
        {
            return GetResourceValueByFormat(locale, listId, itemId, LIST_ITEM_NAME_FORMAT);
        }

        /// <summary>
        /// Gets the description of the list item
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="listId">List identifier</param>
        /// <param name="itemId">List item identifier</param>
        /// <returns>Description of list item</returns>
        private static string GetListItemDescription(string locale, string listId, string itemId)
        {
            return GetResourceValueByFormat(locale, listId, itemId, LIST_ITEM_DESCRIPTION_FORMAT);
        }

        /// <summary>
        /// Gets the resource of the list item
        /// </summary>
        /// <param name="locale">Code of locale</param>
        /// <param name="listId">List identifier</param>
        /// <param name="itemId">List item identifier</param>
        /// <param name="format">Resource item format</param>
        /// <returns>Resource value of the list item</returns>
        private static string GetResourceValueByFormat(string locale, string listId, string itemId, string format)
        {
            return LocalizedResourceManager.GetResourceByLocale(locale, LIST_CATEGORY_NAME,
                string.Format(format, listId, itemId));
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check locale properties
        /// </summary>
        /// <param name="locale">Locale information</param>
        private void VerifyLocaleDto(LocaleDto locale)
        {
            Verify.NotNull(locale, "locale");
            Verify.RaiseWhenFailed();

            Verify.NotNull(locale.Code, "locale.Code");
            Verify.Matches(locale.Code, "^[a-z][a-z](-[a-z][a-z])?$", "locale.Code");
            Verify.NotNullOrEmpty(locale.DisplayName, "locale.DisplayName");
            Verify.MaxLength(locale.DisplayName, 128, "locale.DisplayName");
            Verify.RaiseWhenFailed();
        }

        #endregion
    }
}