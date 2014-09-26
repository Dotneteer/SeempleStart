using System.Collections.Generic;
using Seemplest.Core.DataAccess.DataServices;

namespace SeemplestBlocks.Core.AppConfig.DataAccess
{
    /// <summary>
    /// Ez az interfész a konfigurációs építőelem adatelérés műveleteit definiálja.
    /// </summary>
    public interface IConfigurationDataOperations: IDataAccessOperation
    {
        /// <summary>
        /// Lekérdezi a nyelvi környezetek listáját
        /// </summary>
        /// <returns>Az összes nyelvi környezet listája</returns>
        IList<LocaleRecord> FetchAllLocales();

        /// <summary>
        /// Kód alapján lekérdezi a nyelvi környezeteket
        /// </summary>
        /// <param name="code">A keresett kód</param>
        /// <returns>A kódhoz tartozó nyelvi környezet</returns>
        LocaleRecord GetByCode(string code);

        /// <summary>
        /// Név alapján lekérdezi a nyelvi környezeteket
        /// </summary>
        /// <param name="name">A keresett név</param>
        /// <returns>A névhez tartozó nyelvi környezet</returns>
        LocaleRecord GetByName(string name);

        /// <summary>
        /// Beszúr egy új nyelvi környezetet 
        /// </summary>
        /// <param name="record">Nyelvi környezet</param>
        void InsertLocale(LocaleRecord record);

        /// <summary>
        /// Módosítja az adatbázisban az adott nyelvi környezetet
        /// </summary>
        /// <param name="record">Nyelvi környezet</param>
        void UpdateLocale(LocaleRecord record);

        /// <summary>
        /// Törli az adatbázisból az adott nyelvi környezetet
        /// </summary>
        /// <param name="code">A nyelvi környezet azonosítója</param>
        void DeleteLocale(string code);

        /// <summary>
        /// Lekérdezi az összes erőforrás kategóriát
        /// </summary>
        /// <returns>Az erőforrás kategóriák listája</returns>
        List<CategoryData> FetchAllCategories();
            
        /// <summary>
        /// Lekérdezi az adott kategóriába tartozó erőforrásokat
        /// </summary>
        /// <param name="locale">Kultúrkör információ</param>
        /// <param name="category">Erőforrás kategória</param>
        /// <returns>
        /// Az adott kategóriába tartozó erőforrások
        /// </returns>
        List<LocalizedResourceRecord> GetLocalizedResourcesByCategory(string locale, string category);

        /// <summary>
        /// Törli az adott nyelvi környezethez tartozó erőforrásokat
        /// </summary>
        /// <param name="code">A nyelvi környezet azonosítója</param>
        void DeleteResourcesOfLocale(string code);

        /// <summary>
        /// Törli a cél nyelvi környezetből azokat az erőforrásokat, amelyek már léteznek a forrás
        /// nyelvi környezetben
        /// </summary>
        /// <param name="sourceLocale">Forrás környezet</param>
        /// <param name="targetLocale">Cél környezet</param>
        void DeleteExistingSourceResourcesFromTarget(string sourceLocale, string targetLocale);

        /// <summary>
        /// Átmásolja a forrás nyelvi környezet erőforrásait a cél erőforrásba, a forrásban lévő
        /// értékeket használva
        /// </summary>
        /// <param name="sourceLocale">Forrás környezet</param>
        /// <param name="targetLocale">Cél környezet</param>
        void CloneResources(string sourceLocale, string targetLocale);

        /// <summary>
        /// Átmásolja a forrás nyelvi környezet erőforrásait a cél erőforrásba, az itt megadott
        /// alapértelmezett értéket használva
        /// </summary>
        /// <param name="sourceLocale">Forrás környezet</param>
        /// <param name="targetLocale">Cél környezet</param>
        /// <param name="defaultValue">Alapértelmezett érték</param>
        void CloneResourcesWithDefault(string sourceLocale, string targetLocale, string defaultValue);

        /// <summary>
        /// Lekérdezi az adatbázisban lévő összes konfigurációs értéket
        /// </summary>
        /// <returns>Az összes konfigurációs érték listája</returns>
        List<ConfigurationValueRecord> GetAllConfigurationValues();

        /// <summary>
        /// Lekérdezi az adatbázisban lévő konfigurációs értéket kategória és kulcs alapján
        /// </summary>
        /// <param name="versionId">A változat azonosítója</param>
        /// <param name="category">Kategória</param>
        /// <param name="key">Konfigurációs kulcs</param>
        /// <returns>A keresett konfigurációs rekord</returns>
        ConfigurationValueRecord GetConfigurationValueByKey(int versionId, string category, string key);

        /// <summary>
        /// Beszúrja az adatbázisba az adott konfigurációs értéket
        /// </summary>
        /// <param name="value">Konfigurációs érték adatai</param>
        void InsertConfigurationValue(ConfigurationValueRecord value);

        /// <summary>
        /// Módosítja az adatbázisban az adott konfigurációs értéket
        /// </summary>
        /// <param name="value">Konfigurációs érték adatai</param>
        void UpdateConfigurationValue(ConfigurationValueRecord value);

        /// <summary>
        /// Kitörli az adatbázisból az adott konfigurációs értéket
        /// </summary>
        /// <param name="versionId">A változat azonosítója</param>
        /// <param name="category">Kategória</param>
        /// <param name="key">Konfigurációs kulcs</param>
        void DeleteConfigurationValue(int versionId, string category, string key);

        /// <summary>
        /// Lekérdezi az aktuális konfigurációs változatot
        /// </summary>
        /// <returns>Az aktuális konfigurációs váltoxat</returns>
        CurrentConfigurationVersionRecord GetCurrentConfigurationVersion();

        /// <summary>
        /// Lekérdezi az adott azonosítójú lista definícióját
        /// </summary>
        /// <param name="listId">A lista azonosítója</param>
        /// <returns>A lista definíciója, ha megtaláltuk, egyébként null</returns>
        ListDefinitionRecord GetListDefinitionById(string listId);

        /// <summary>
        /// Lekérdezi az adott listához tartodó elemek definícióit
        /// </summary>
        /// <param name="listId">A lista azonosítója</param>
        /// <returns>A listához tartozó elemek</returns>
        List<ListItemDefinitionRecord> GetListItems(string listId);
    }
}