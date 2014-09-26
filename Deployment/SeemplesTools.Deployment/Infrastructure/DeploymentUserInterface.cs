namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály a telepítés során használt felhasználói felületet definiálja
    /// </summary>
    public static class DeploymentUserInterface
    {
        /// <summary>
        /// Az aktuális felhasználói felület példányt adja vissza.
        /// </summary>
        public static IDeploymentUserInterface Current { get; set; }
    }
}
