using System;
using System.Linq;
using RTI.DataBase.Updater.Config;
using System.IO;
using System.Runtime.CompilerServices;

namespace RTI.DataBase.Util
{
    public static class Logger
    {
        public static string LogFileFullPath { get { return _logFullPath; } private set { } }
        public static string LogFolder { get { return _logPath;  } private set { } }
        private static string _logPath;
        private static string _logFullPath;
        private static DateTime _date = new DateTime();

        /// <summary>
        /// Initialize a new 
        /// Logging session.
        /// </summary>
        static Logger()
        {
            _date = DateTime.UtcNow;
            string dateString = _date.ToString("ddmmyyyyHHss");
            string logFileLocation = Log.Settings.LogFolderLocation;
            string logFileName = Path.GetFileNameWithoutExtension(Log.Settings.LogFileName);

            if (logFileLocation.ToLower() == "default")
                _logPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "logs", dateString);
            else
                _logPath = Path.Combine(logFileLocation, dateString);

            string logFullPath = Path.Combine(_logPath, logFileName + ".txt");

            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);

            if (!File.Exists(logFullPath))
                File.Create(logFullPath).Close();

            _logFullPath = logFullPath;
        }

        /// <summary>
        /// Writes a message to 
        /// the error log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <param name="args"></param>
        public static void WriteToLog(string message, Priority priority = Priority.Info, params object[] args)
        {
            Priority logLevel = Log.Settings.LogLevel;
            if (priority > logLevel)
            {
                string msg = string.Join(": ", new string[] { priority.ToString(), message });
                WriteToLogFile(msg);
                Console.WriteLine(msg, args);
            }
        }

        /// <summary>
        /// Write an Exception to 
        /// the error log. 
        /// </summary>
        public static void WriteErrorToLog(Exception ex, string message = "", Priority priority = Priority.Error, [CallerMemberName]string method = "", [CallerLineNumber] int line  = -999, [CallerFilePath] string ClassPath = "")
        {
            string msg = (!string.IsNullOrEmpty(message)) ? string.Join(": ", new string[] { "ErrorMessage", message }) : "";
            string exceptionMessage = ex?.Message ?? String.Empty;
            string innerExceptionMessage = ex?.InnerException?.Message ?? String.Empty;
            string stackTrace = ex?.StackTrace ?? String.Empty;
            string memberData = "@" + ((ClassPath != "") ? "Class '" + Path.GetFileName(ClassPath) + "'": "") +
                                      ((line != -999) ? ", Line Number " + line : "") + 
                                      ((method != "") ? " Method " + method + "()" : "");

            string error = msg +
                           (string.IsNullOrEmpty(exceptionMessage) ? "" : "\r\nExceptionMessage: " + exceptionMessage) + 
                           (string.IsNullOrEmpty(innerExceptionMessage) ? "" : "\r\nInnerExceptionMessage: " + innerExceptionMessage) +
                           (string.IsNullOrEmpty(stackTrace) ? "" : "\r\nStackTrace: " + stackTrace) +
                           (string.IsNullOrEmpty(memberData) ? "" : "\r\nCallingMember: " + memberData);

            int maxLineLength = error.Split(new string[] { "\r\n", Environment.NewLine }, StringSplitOptions.None).ToArray().OrderByDescending(a => a.Length).FirstOrDefault().Length;
            string headerFooter = new string('-', maxLineLength);
            error = Environment.NewLine +  headerFooter + Environment.NewLine + error + Environment.NewLine + headerFooter;

            WriteToLog(error, priority);
        }

        /// <summary>
        /// Write text to the 
        /// log file.
        /// </summary>
        /// <param name="message"></param>
        private static void WriteToLogFile(string message)
        {
            if (!File.Exists(_logFullPath))
                File.Create(_logFullPath);

            using (StreamWriter writer = new StreamWriter(_logFullPath, true))
                writer.WriteLine(message);
        }     
    }
}
