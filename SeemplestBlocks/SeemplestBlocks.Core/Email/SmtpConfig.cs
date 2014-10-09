using System.Runtime.CompilerServices;
using SeemplestBlocks.Core.AppConfig;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This class defines the configuration of the SMTP service to manage emails
    /// </summary>
    public static class SmtpConfig
    {
        private static readonly ConfigurationPropertyValueReader s_Reader =
            new ConfigurationPropertyValueReader();

        /// <summary>
        /// Is email sending enabled?
        /// </summary>
        public static bool Enabled
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<bool>(); }
        }

        /// <summary>
        /// Should be SMTP exceptions thrown?
        /// </summary>
        public static bool ThrowException
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<bool>(); }
        }

        /// <summary>
        /// SMTP server name
        /// </summary>
        public static string SmtpServer
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }


        /// <summary>
        /// SMTP port number
        /// </summary>
        public static int PortNumber
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        /// <summary>
        /// Do we use SSL?
        /// </summary>
        public static bool UseSsl
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<bool>(); }
        }

        /// <summary>
        /// Should we use SMTP authentication?
        /// </summary>
        public static bool SmtpAuth
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<bool>(); }
        }

        /// <summary>
        /// User name to access SMTP server
        /// </summary>
        public static string UserName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// SMTP access password
        /// </summary>
        public static string Password
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// Email sending interval (in milliseconds)
        /// </summary>
        public static int SendInterval
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        /// <summary>
        /// Maximum number of emails to be sent in one loop
        /// </summary>
        public static int EmailCount
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        /// <summary>
        /// Mximum count of retries
        /// </summary>
        public static int MaxRetry
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        /// <summary>
        /// Minutes between retries
        /// </summary>
        public static int RetryMinutes
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<int>(); }
        }

        /// <summary>
        /// Default sender email address
        /// </summary>
        public static string EmailFromAddr
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// Default sender name
        /// </summary>
        public static string EmailFromName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }
    }
}