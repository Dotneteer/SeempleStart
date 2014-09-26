namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az interfész a felhasználói felület absztrakt interfészét definiálja
    /// </summary>
    public interface IDeploymentUserInterface
    {
        /// <summary>
        /// Az előrehaladás jelzésének indítása
        /// </summary>
        void StartProgress();

        /// <summary>
        /// Az előrehaladás jelzése
        /// </summary>
        /// <param name="progress">Az előrehaladás mértéke</param>
        /// <param name="progressText">Az előrehaladás aktuális fázisa</param>
        void SetProgress(int progress, string progressText);

        /// <summary>
        /// Az előrehaladás jelzésének befejezése
        /// </summary>
        void EndProgress();
    }
}
