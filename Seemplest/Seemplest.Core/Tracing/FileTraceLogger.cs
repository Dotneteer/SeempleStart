using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;

namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This class logs trace messages into a file.
    /// </summary>
    /// <remarks>
    /// Message are collected in a queue and are written into a file in the background.
    /// </remarks>
    public class FileTraceLogger : TraceLoggerBase, IDisposable
    {
        private const int MAX_ENTRIES_DEFAULT = 10;

        private readonly ConcurrentQueue<TraceLogItem> _entryQueue = 
            new ConcurrentQueue<TraceLogItem>();
        private bool _isDisposing;
        private bool _isFlushing;

        /// <summary>
        /// Creates the specified file tracer object
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="rootFolder"></param>
        /// <param name="folderPattern"></param>
        /// <param name="fileNamePrefixPattern"></param>
        /// <param name="fileNameSuffixPattern"></param>
        /// <param name="timestampFormat"></param>
        /// <param name="flushAfter"></param>
        public FileTraceLogger(string fileName, string rootFolder, 
            string folderPattern = null, 
            string fileNamePrefixPattern = null, 
            string fileNameSuffixPattern = null, 
            string timestampFormat = null,
            int flushAfter = MAX_ENTRIES_DEFAULT)
        {
            FileName = fileName;
            RootFolder = rootFolder;
            FolderPattern = folderPattern ?? "";
            FileNamePrefixPattern = fileNamePrefixPattern ?? "";
            FileNameSuffixPattern = fileNameSuffixPattern ?? "";
            TimestampFormat = timestampFormat ?? "yyyy.MM.dd hh:mm:ss.fff";
            FlushAfter = flushAfter;
            _isFlushing = false;
        }

        /// <summary>
        /// Gets or sets the root folder where log files should be saved.
        /// </summary>
        public string RootFolder { get; private set; }

        /// <summary>
        /// Gets or sets the pattern to create subfolders.
        /// </summary>
        public string FolderPattern { get; private set; }

        /// <summary>
        /// Gets or sets the file name used for logging.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets the pattern to set up the file name prefix.
        /// </summary>
        public string FileNamePrefixPattern { get; private set; }

        /// <summary>
        /// Gets or sets the pattern to set up the file name suffix.
        /// </summary>
        public string FileNameSuffixPattern { get; private set; }

        /// <summary>
        /// The format of the timestamp in the output file.
        /// </summary>
        public string TimestampFormat { get; private set; }

        /// <summary>
        /// The number of log entries to flush after
        /// </summary>
        public int FlushAfter { get; private set; }

        /// <summary>
        /// Override to specify how the trace entry should be logged.
        /// </summary>
        /// <param name="item">Trace entry</param>
        protected override void DoLog(TraceLogItem item)
        {
            if (_isDisposing) return;
            _entryQueue.Enqueue(item);
            if (_entryQueue.Count >= FlushAfter)
            {
                Flush();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _isDisposing = true;
            Flush();
        }

        /// <summary>
        /// Flushes the content of trace message queue into the output file
        /// </summary>
        private void Flush()
        {
            if (_isFlushing) return;
            try
            {
                _isFlushing = true;
                ComplexFlush();
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // --- This exception is intentionally drained
            }
            finally
            {
                _isFlushing = false;
            }
        }

        /// <summary>
        /// Manages the complex logging scenario
        /// </summary>
        private void ComplexFlush()
        {
            string lastFullPath = null;
            StreamWriter currentStream = null;

            var count = _entryQueue.Count;
            for (var i = 0; i < count; i++)
            {
                TraceLogItem item;
                if (!_entryQueue.TryDequeue(out item)) continue;

                // -- Calculate the new full log file name according to the timestamp
                var fullPath = GetFullFilePath(item);

                // --- Check whether a new file path should be created or not
                if (lastFullPath != fullPath)
                {
                    if (currentStream != null)
                    {
                        currentStream.Flush();
                        currentStream.Close();
                    }
                    var dirName = Path.GetDirectoryName(fullPath);
                    if (dirName != null && !Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    var stream = File.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    currentStream = new StreamWriter(stream);
                    lastFullPath = fullPath;
                }
                LogSingleItem(currentStream, item);
            }
            if (currentStream == null) return;
            currentStream.Flush();
            currentStream.Close();
        }

        /// <summary>
        /// Gets the full path of the file the specified entry should be logged to.
        /// </summary>
        /// <param name="entry">Log entry</param>
        /// <returns>The full path of the log file to be used.</returns>
        protected virtual string GetFullFilePath(TraceLogItem entry)
        {
            // -- Calculate the new full log file name according to the timestamp
            // ReSharper disable FormatStringProblem
            var subfolder =
                string.IsNullOrWhiteSpace(FolderPattern)
                    ? String.Empty
                    : string.Format(CultureInfo.InvariantCulture, "{0:" + FolderPattern + "}", entry.TimestampUtc);
            var filePrefix =
                string.IsNullOrWhiteSpace(FileNamePrefixPattern)
                    ? String.Empty
                    : string.Format(CultureInfo.InvariantCulture, "{0:" + FileNamePrefixPattern + "}", entry.TimestampUtc);
            var fileSuffix =
                string.IsNullOrWhiteSpace(FileNameSuffixPattern)
                    ? String.Empty
                    : string.Format(CultureInfo.InvariantCulture, "{0:" + FileNameSuffixPattern + "}", entry.TimestampUtc);
            var filebase = Path.GetFileNameWithoutExtension(FileName);
            var extension = Path.GetExtension(FileName);
            var fullFileName = filePrefix + filebase + fileSuffix + extension;
            return Path.Combine(RootFolder, Path.Combine(subfolder, fullFileName));
            // ReSharper restore FormatStringProblem
        }

        private static void LogSingleItem(TextWriter writer, TraceLogItem item)
        {
            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", 
                item.TimestampUtc, 
                item.Type, 
                item.ServerName, 
                item.ThreadId, 
                item.OperationType, 
                item.Message, 
                item.DetailedMessage);
        }
    }
}