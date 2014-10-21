using Seemplest.Core.WindowsEventLog;
using SeemplestBlocks.Core.Diagnostics;
using SeemplestBlocks.Core.Email;

namespace SeemplestCloud.WebClient
{
    /// <summary>
    /// Ez az osztály felelős a háttérben futó feladatok konfigurálásáért
    /// </summary>
    public static class BackgroundTaskConfig
    {
        private static readonly EmailTaskProcessor s_EmailProcessor = new EmailTaskProcessor();

        /// <summary>
        /// Elindítja háttérben futó feldolgozásokat
        /// </summary>
        public static void StartBackgroundProcessing()
        {
            Tracer.Start();
            WindowsEventLogger.RedirectLogTo(Tracer.LoggerInstance);
            s_EmailProcessor.SetContext(new EmailTaskExecutionContext());
            s_EmailProcessor.Start();
        }

        /// <summary>
        /// Leállítja a háttérben futó feldolgozásokat
        /// </summary>
        public static void StopBackgroundProcessing()
        {
            s_EmailProcessor.Stop();
            Tracer.Stop();
        }
    }
}