namespace SeemplesTools.Deployment.Common
{
    /// <summary>
    /// Ez az osztály a változatok kezeléséhez kínál hasznos műveleteket
    /// </summary>
    public static class VersionHelper
    {
        private static readonly DatabaseVersion s_DatabaseVersion = new DatabaseVersion(0, 6);

        /// <summary>
        /// Az adatbázis verziója
        /// </summary>
        public static DatabaseVersion DatabaseVersion
        {
            get { return s_DatabaseVersion; }
        }
    }
}
