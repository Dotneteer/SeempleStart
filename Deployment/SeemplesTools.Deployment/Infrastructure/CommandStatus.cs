namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// A parancsok lehetséges állapotai
    /// </summary>
    public enum CommandStatus
    {
        /// <summary>Még inicializálás alatt</summary>
        Initializing,
        /// <summary>Már futtatható</summary>
        Runnable,
        /// <summary>Sikeresen végre lett hajtva</summary>
        Succeeded,
        /// <summary>Meghiúsult a végrehajtás</summary>
        Failed,
        /// <summary>Véglegesített</summary>
        Committed,
        /// <summary>A véglegesítés meghiúsult</summary>
        CommitFailed,
        /// <summary>Visszagörgetett</summary>
        RolledBack,
        /// <summary>A visszagörgetés meghiúsult</summary>
        RollbackFailed
    }
}
