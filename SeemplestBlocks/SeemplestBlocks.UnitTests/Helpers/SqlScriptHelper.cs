using System;
using System.IO;
using System.Reflection;
using Seemplest.MsSql.DataAccess;

namespace SeemplestBlocks.UnitTests.Helpers
{
    /// <summary>
    /// Ez az osztály a tesztesetek kezeléséhez kapcsolódó SQL szolgáltatásokat valósít meg
    /// </summary>
    public static class SqlScriptHelper
    {
        private const string DB_CONN = "connStr=Core";
        private const string RESOURCE_FOLDER = "SqlScripts";

        /// <summary>
        /// Elhozza az adott erőforrásban levő szkript tartalmát
        /// </summary>
        /// <param name="resourceName">A script erőforrás neve</param>
        /// <returns>A szkript tartalma</returns>
        public static string GetScript(string resourceName)
        {
            var resourceFullName = String.Format("{0}.{1}.{2}",
                Assembly.GetExecutingAssembly().GetName().Name,
                RESOURCE_FOLDER,
                resourceName);
            var resMan = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFullName);
            // ReSharper disable once AssignNullToNotNullAttribute
            var tr = new StreamReader(resMan);
            return tr.ReadToEnd();
        }

        /// <summary>
        /// Lefuttatja az adott erőforrásban lévő scriptet
        /// </summary>
        /// <param name="resourceName">A script erőforrás neve</param>
        /// <param name="connInfo">Opcionális kapcsolódási informació</param>
        public static void RunScript(string resourceName, string connInfo = DB_CONN)
        {
            var script = GetScript(resourceName);
            var dc = new SqlDatabase(connInfo);
            dc.Execute(script);
        }
    }
}