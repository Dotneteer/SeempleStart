using System.Runtime.CompilerServices;
using Seemplest.Core.Tracing;
using SeemplestBlocks.Core.AppConfig;
using System;

namespace SeemplestBlocks.Core.Diagnostics
{
    /// <summary>
    /// This class provides configuration values for the Tracer service
    /// </summary>
    public static class TracerConfiguration
    {
        private static readonly ConfigurationPropertyValueReader s_Reader =
            new ConfigurationPropertyValueReader();

        /// <summary>
        /// Is logging enabled?
        /// </summary>
        public static bool Enabled
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<bool>(); }
        }

        /// <summary>
        /// Folder that stores log information
        /// </summary>
        public static string LogFolder
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// File that stores log information
        /// </summary>
        public static string LogFile
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// Date pattern to define the full folder name
        /// </summary>
        public static string FolderPattern
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// Date pattern to define the file name suffix
        /// </summary>
        public static string FileSuffixPattern
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get { return s_Reader.GetValue<string>(); }
        }

        /// <summary>
        /// Level of tracing
        /// </summary>
        public static TraceLogItemType LogLevel
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get
            {
                TraceLogItemType logType;
                return Enum.TryParse(s_Reader.GetValue<string>(), true, out logType)
                    ? logType
                    : TraceLogItemType.Error;
            }
        }
    }
}