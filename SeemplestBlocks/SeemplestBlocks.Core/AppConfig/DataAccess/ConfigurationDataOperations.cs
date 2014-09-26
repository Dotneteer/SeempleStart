using System.Collections.Generic;
using Seemplest.MsSql.DataAccess;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    /// <summary>
    /// Ez az osztály a konfigurációs építőelem adatelérés műveleteit valósítja meg.
    /// </summary>
    public class ConfigurationDataOperations : SqlDataAccessOperationBase, IConfigurationDataOperations
    {
        private const int CURRENT_VERSION_KEY = 1;

        /// <summary>
        /// Az adatelérés objektumot a megadott adatbázis kapcsolódási információval inicializálja
        /// </summary>
        /// <param name="connectionOrName">Adatbázis kapcsolódási információ</param>
        public ConfigurationDataOperations(string connectionOrName) : base(connectionOrName)
        {
        }

        /// <summary>
        /// Lekérdezi a nyelvi környezetek listáját
        /// </summary>
        /// <returns>Az összes nyelvi környezet listája</returns>
        public IList<LocaleRecord> FetchAllLocales()
        {
            return Operation(ctx => ctx.Fetch<LocaleRecord>());
        }

        /// <summary>
        /// Kód alapján lekérdezi a nyelvi környezeteket
        /// </summary>
        /// <param name="code">A keresett kód</param>
        /// <returns>A kódhoz tartozó nyelvi környezet</returns>
        public LocaleRecord GetByCode(string code)
        {
            return Operation(ctx => ctx.FirstOrDefault<LocaleRecord>("where [Code]=@0", code));
        }

        /// <summary>
        /// Név alapján lekérdezi a nyelvi környezeteket
        /// </summary>
        /// <param name="name">A keresett név</param>
        /// <returns>A névhez tartozó nyelvi környezet</returns>
        public LocaleRecord GetByName(string name)
        {
            return Operation(ctx => ctx.FirstOrDefault<LocaleRecord>("where [DisplayName]=@0", name));
        }

        /// <summary>
        /// Beszúr egy új nyelvi környezetet 
        /// </summary>
        /// <param name="record">Nyelvi környezet</param>
        public void InsertLocale(LocaleRecord record)
        {
            Operation(ctx => ctx.Insert(record));
        }

        /// <summary>
        /// Módosítja az adatbázisban az adott nyelvi környezetet
        /// </summary>
        /// <param name="record">Nyelvi környezet</param>
        public void UpdateLocale(LocaleRecord record)
        {
            Operation(ctx => ctx.Update(record));
        }

        /// <summary>
        /// Törli az adatbázisból az adott nyelvi környezetet
        /// </summary>
        /// <param name="code">A nyelvi környezet azonosítója</param>
        public void DeleteLocale(string code)
        {
            Operation(ctx => ctx.Execute(
                "delete from [Config].[Locale] where [Code]=@0", code));
        }

        /// <summary>
        /// Lekérdezi az összes erőforrás kategóriát
        /// </summary>
        /// <returns>Az erőforrás kategóriák listája</returns>
        public List<CategoryData> FetchAllCategories()
        {
            return
                Operation(ctx => ctx.Fetch<CategoryData>(
                    "select distinct [Category] from [Config].[LocalizedResource]"));
        }

        /// <summary>
        /// Lekérdezi az adott kategóriába tartozó erőforrásokat
        /// </summary>
        /// <param name="locale">Kultúrkör információ</param>
        /// <param name="category">Erőforrás kategória</param>
        /// <returns>
        /// Az adott kategóriába tartozó erőforrások
        /// </returns>
        public List<LocalizedResourceRecord> GetLocalizedResourcesByCategory(string locale, string category)
        {
            return Operation(ctx => ctx.Fetch<LocalizedResourceRecord>(
                "where [Locale]=@0 and [Category]=@1", locale, category));
        }

        /// <summary>
        /// Törli az adott nyelvi környezethez tartozó erőforrásokat
        /// </summary>
        /// <param name="code">A nyelvi környezet azonosítója</param>
        public void DeleteResourcesOfLocale(string code)
        {
            Operation(ctx => ctx.Execute(
                "delete from [Config].[LocalizedResource] where [Locale]=@0", code));
        }

        void IConfigurationDataOperations.DeleteExistingSourceResourcesFromTarget(string sourceLocale, string targetLocale)
        {
            const string QUERY =
                @"delete [Config].[LocalizedResource]
                  from [Config].[LocalizedResource] target
                  where [ResourceKey] in 
                    (select [ResourceKey] from [Config].[LocalizedResource] source 
                        where source.Locale = @0 
	                    and source.Category = target.Category 
	                    and source.ResourceKey = target.ResourceKey)
                    and Locale = @1";
            Operation(ctx => ctx.Execute(QUERY, sourceLocale, targetLocale));
        }

        /// <summary>
        /// Átmásolja a forrás nyelvi környezet erőforrásait a cél erőforrásba, a forrásban lévő
        /// értékeket használva
        /// </summary>
        /// <param name="sourceLocale">Forrás környezet</param>
        /// <param name="targetLocale">Cél környezet</param>
        public void CloneResources(string sourceLocale, string targetLocale)
        {
            const string QUERY =
                 @"insert [Config].[LocalizedResource]
                   select @1, source.Category, source.ResourceKey, source.Value
                   from [Config].[LocalizedResource] source
                   where 
                       source.Locale = @0
                       and not exists (select * from [Config].[LocalizedResource] dest 
	                       where dest.Locale = @1 
		                   and dest.Category = source.Category 
		                   and dest.ResourceKey = source.ResourceKey)";
            Operation(ctx => ctx.Execute(QUERY, sourceLocale, targetLocale));
        }

        /// <summary>
        /// Átmásolja a forrás nyelvi környezet erőforrásait a cél erőforrásba, az itt megadott
        /// alapértelmezett értéket használva
        /// </summary>
        /// <param name="sourceLocale">Forrás környezet</param>
        /// <param name="targetLocale">Cél környezet</param>
        /// <param name="defaultValue">Alapértelmezett érték</param>
        public void CloneResourcesWithDefault(string sourceLocale, string targetLocale, string defaultValue)
        {
            const string QUERY =
                 @"insert [Config].[LocalizedResource]
                   select @1, source.Category, source.ResourceKey, @2
                   from [Config].[LocalizedResource] source
                   where 
                       source.Locale = @0
                       and not exists (select * from [Config].[LocalizedResource] dest 
	                       where dest.Locale = @1 
		                   and dest.Category = source.Category 
		                   and dest.ResourceKey = source.ResourceKey)";
            Operation(ctx => ctx.Execute(QUERY, sourceLocale, targetLocale, defaultValue));
        }

        /// <summary>
        /// Lekérdezi az adatbázisban lévő összes konfigurációs értéket
        /// </summary>
        /// <returns></returns>
        public List<ConfigurationValueRecord> GetAllConfigurationValues()
        {
            return Operation(ctx => ctx.Fetch<ConfigurationValueRecord>());
        }

        /// <summary>
        /// Lekérdezi az adatbázisban lévő konfigurációs értéket kategória és kulcs alapján
        /// </summary>
        /// <param name="versionId">A változat azonosítója</param>
        /// <param name="category">Kategória</param>
        /// <param name="key">Konfigurációs kulcs</param>
        /// <returns>A keresett konfigurációs rekord</returns>
        public ConfigurationValueRecord GetConfigurationValueByKey(int versionId, string category, string key)
        {
            return Operation(ctx => ctx.FirstOrDefault<ConfigurationValueRecord>(
                "where [VersionId]=@0 and [Category]=@1 and [ConfigKey]=@2", versionId, category, key));
        }

        /// <summary>
        /// Beszúrja az adatbázisba az adott konfigurációs értéket
        /// </summary>
        /// <param name="value">Konfigurációs érték adatai</param>
        public void InsertConfigurationValue(ConfigurationValueRecord value)
        {
            Operation(ctx => ctx.Insert(value));
        }

        /// <summary>
        /// Módosítja az adatbázisban az adott konfigurációs értéket
        /// </summary>
        /// <param name="value">Konfigurációs érték adatai</param>
        public void UpdateConfigurationValue(ConfigurationValueRecord value)
        {
            Operation(ctx => ctx.Update(value));
        }

        /// <summary>
        /// Kitörli az adatbázisból az adott konfigurációs értéket
        /// </summary>
        /// <param name="versionId">A változat azonosítója</param>
        /// <param name="category">Kategória</param>
        /// <param name="key">Konfigurációs kulcs</param>
        public void DeleteConfigurationValue(int versionId, string category, string key)
        {
            Operation(ctx => ctx.Execute(
                "delete from [Config].[ConfigurationValue] where [VersionId]=@0 and [Category]=@1 and [ConfigKey]=@2",
                versionId, category, key));

        }

        /// <summary>
        /// Lekérdezi az aktuális konfigurációs változatot
        /// </summary>
        /// <returns>Az aktuális konfigurációs váltoxat</returns>
        public CurrentConfigurationVersionRecord GetCurrentConfigurationVersion()
        {
            return Operation(ctx => ctx.FirstOrDefault<CurrentConfigurationVersionRecord>(
                "where [Id]=@0", CURRENT_VERSION_KEY));
        }

        /// <summary>
        /// Lekérdezi az adott azonosítójú lista definícióját
        /// </summary>
        /// <param name="listId">A lista azonosítója</param>
        /// <returns>A lista definíciója, ha megtaláltuk, egyébként null</returns>
        public ListDefinitionRecord GetListDefinitionById(string listId)
        {
            return Operation(ctx => ctx.FirstOrDefault<ListDefinitionRecord>(
                "where [Id]=@0", listId));
        }

        /// <summary>
        /// Lekérdezi az adott listához tartodó elemek definícióit
        /// </summary>
        /// <param name="listId">A lista azonosítója</param>
        /// <returns>A listához tartozó elemek</returns>
        public List<ListItemDefinitionRecord> GetListItems(string listId)
        {
            return Operation(ctx => ctx.Fetch<ListItemDefinitionRecord>(
                "where [ListId]=@0", listId));
        }
    }
}