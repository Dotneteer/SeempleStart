using System.Runtime.CompilerServices;
using SeemplestBlocks.Core.AppConfig;

namespace SeemplestCloud.Services.SubscriptionService
{
    /// <summary>
    /// This class provides subscription configuration settings
    /// </summary>
    public static class SubscriptionConfig
    {
        private static readonly ConfigurationPropertyValueReader s_Reader =
            new ConfigurationPropertyValueReader();

        /// <summary>
        /// Invitation link prefix
        /// </summary>
        public static string InvitationLinkPrefix
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }
    }
}