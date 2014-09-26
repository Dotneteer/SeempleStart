namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály egy adatbázis név és -változat párt ír le.
    /// </summary>
    public class DatabaseNameVersionPair
    {
        /// <summary>
        /// Az adatbázis neve
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Az adatbázis változata
        /// </summary>
        public DatabaseVersion Version { get; private set; }

        /// <summary>
        /// A megadott paraméterekkel inicializálja az objektumot
        /// </summary>
        /// <param name="name">Az adatbázis neve</param>
        /// <param name="version">Az adatbázis változata</param>
        public DatabaseNameVersionPair(string name, DatabaseVersion version)
        {
            Name = name;
            Version = version;
        }
    }
}
