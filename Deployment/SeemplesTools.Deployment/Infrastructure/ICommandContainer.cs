namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az interfész egy parancskonténer viselkedését írja le.
    /// </summary>
    public interface ICommandContainer
    {
        /// <summary>
        /// Meg lett szakítva a parancsok végrehajtása?
        /// </summary>
        bool Cancelled { get; }

        /// <summary>
        /// A parancsok előrehaladottságának jelzése
        /// </summary>
        /// <param name="command">A parancsot leíró objektum</param>
        /// <param name="progressText">Az aktuális előrehaladottsági fázis</param>
        /// <param name="commandProgress">Az előrehaladás mértéke</param>
        void ReportProgress(Command command, string progressText, int commandProgress = 0);
    }
}
