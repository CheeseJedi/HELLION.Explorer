using System;
using System.IO;
using System.Text;

namespace HELLION.DataStructures.Utilities
{
    public class LogFileHandler
    {
        #region Constructor

        public LogFileHandler(LoggingOperationType mode = LoggingOperationType.ConsoleOnly, FileInfo logFile = null, LoggingLevel defaultLevel = LoggingLevel.Default)
        {
            Mode = mode;
            LogFile = logFile;
            if (Mode == LoggingOperationType.LogFileOnly ||
                Mode == LoggingOperationType.ConsoleAndLogFile)
            {
                if (LogFile == null) throw new NullReferenceException("Operation mode requires a log file but none was specified. LogFile was null.");
                if (LogFile.Exists) throw new InvalidOperationException("Specified log file already exists.");
            }
            DefaultLevel = defaultLevel;
            // if (Mode == LoggingOperationType.ConsoleAndCachedLogFile)
            _logBuffer = new StringBuilder();

        }

        #endregion

        #region Properties

        public LoggingOperationType Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;

                    switch ( _mode)
                    {
                        case LoggingOperationType.ConsoleAndBufferedLogFile:
                        case LoggingOperationType.ConsoleAndLogFile:
                        case LoggingOperationType.LogFileOnly:
                            // Check validity of LogFile FileInfo
                            if (LogFile == null || !LogFile.Directory.Exists)
                            {
                                throw new InvalidOperationException("Failed to activate logging, problem with log file path.");
                            }


                            break;
                    }


                    // Check the buffer and flush if necessary

                    




                }
            }
        }

        public FileInfo LogFile { get; set; }

        public LoggingLevel DefaultLevel { get; set; }

        #endregion

        #region Methods

        public string GenerateLogFileName(string postfix = "")
        {
            return DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + postfix + ".log";
        }

        /// <summary>
        /// Writes a line of text to the specified file.
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLineToFile(string msg)
        {
            using (StreamWriter sw = new StreamWriter(LogFile.FullName))
            {
                try
                {
                    sw.WriteLine(msg);
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        public void WriteLine(string msg, LoggingLevel msgLvl = LoggingLevel.Default)
        {
            string logLine = DateTime.UtcNow.ToString("yyyyMMddTHHmmss: ") + msg + Environment.NewLine;

            _logBuffer.Append(logLine);

            // First switch - for writing to console.
            switch (Mode)
            {
                case LoggingOperationType.ConsoleOnly:
                case LoggingOperationType.ConsoleAndLogFile:

                    if (msgLvl == LoggingLevel.Default ||
                       (msgLvl == LoggingLevel.Verbose && DefaultLevel == LoggingLevel.Verbose))
                    {
                        Console.WriteLine(msg);
                    }
                    break;
                default:
                    break;
            }

            // Second switch - for writing to a file.
            switch (Mode)
            {
                case LoggingOperationType.ConsoleAndLogFile:
                case LoggingOperationType.LogFileOnly:

                    if (msgLvl == LoggingLevel.Default || msgLvl == LoggingLevel.Verbose)
                    {
                        FlushBuffer();
                    }
                    break;
                default:
                    break;
            }

            //if (msgLvl == LoggingLevel.Default)
            //{
            //    Console.WriteLine(logLine);
            //    WriteLineToFile(logLine);
            //}
            //else if (msgLvl == LoggingLevel.Verbose)
            //{
            //    if (DefaultLevel == LoggingLevel.Verbose)
            //    {
            //        Console.WriteLine(logLine);
            //    }
            //    WriteLineToFile(logLine);
            //}

        }

        public void FlushBuffer()
        {
            if (Mode == LoggingOperationType.ConsoleAndBufferedLogFile
                || Mode == LoggingOperationType.ConsoleAndLogFile
                || Mode == LoggingOperationType.LogFileOnly)
            {
                WriteLineToFile(_logBuffer.ToString());
            }
        }

        #endregion

        #region Fields

        private LoggingOperationType _mode;
        private StringBuilder _logBuffer = null;

        #endregion

        #region Enumerations

        public enum LoggingLevel
        {
            Unspecified = 0, // Default.
            Default,
            Verbose
        }

        public enum LoggingOperationType
        {
            Unspecified = 0,
            ConsoleOnly,
            LogFileOnly,
            ConsoleAndLogFile,
            ConsoleAndBufferedLogFile,
        }

        #endregion

    }

}
