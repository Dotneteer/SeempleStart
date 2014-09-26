using System.Collections.Generic;
using System.Data.SqlClient;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály egy adatbázis tranzakciót ír le.
    /// </summary>
    public class DatabaseTransaction
    {
        /// <summary>
        /// A tranzakció elérése az SQL szerveren
        /// </summary>
        public SqlTransaction Transaction { get; set; }
        
        /// <summary>
        /// Az ugyanehhez a tranzakcióhoz kapcsolódó parancsok
        /// </summary>
        public HashSet<Command> Commands { get; private set; }

        /// <summary>
        /// A tranzakció objektum inicializálása
        /// </summary>
        public DatabaseTransaction()
        {
            Commands = new HashSet<Command>();
        }
    }
}
