using System;
using System.Linq;
using RTI.DataBase.Updater.Config;
using System.IO;
using System.Runtime.CompilerServices;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Objects;

namespace RTI.DataBase.Util
{
    public class Logger : ILogger, IFileWriter, IExceptionProcessor
    {
        public string LogFileFullPath { get { return _logFullPath; } private set { } }
        public string LogFolder { get { return _logPath;  } private set { } }
        EmailAlertList ILogger.AlertList { get { return AlertList; } }
        private static string _logPath;
        private static string _logFullPath;
        private static DateTime _date = new DateTime();
        private EmailAlertList AlertList = new EmailAlertList();

        /// <summary>
        /// Initialize a new 
        /// Logging session.
        /// </summary>
        public Logger()
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
        public void WriteMessageToLog(string message, Priority priority = Priority.Info, params object[] args)
        {
            Priority logLevel = Log.Settings.LogLevel;
            if (priority > logLevel)
            {
                string msg = string.Join(": ", new string[] { priority.ToString(), message });
                WriteToFile(msg, _logFullPath);
                Console.WriteLine(msg, args);
            }
        }

        /// <summary>
        /// Write an Exception to 
        /// the error log. 
        /// </summary>
        public void WriteErrorToLog(Exception ex, string message = "", bool addToEmail = false, Priority priority = Priority.Error, [CallerMemberName]string method = "", [CallerLineNumber] int line  = -999, [CallerFilePath] string ClassPath = "")
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

            if (addToEmail)
                ProcessExcption(ex, error, priority);

            WriteMessageToLog(error, priority);
        }

        private static object lockObject = new object();

        /// <summary>
        /// Write text to the 
        /// log file.
        /// </summary>
        /// <param name="message"></param>
        public void WriteToFile(string text, string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            lock (lockObject)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                    writer.WriteLine(text);
            }
        }

        /// <summary>
        /// Returns a new Alert
        /// using data from an Exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public void ProcessExcption(Exception ex, string message = null, Priority priority = Priority.Error)
        {
            Alert alert = new Alert();
            alert.Priority = priority;
            alert.Message = message ?? "";
            alert.ExceptionMessage = ex?.Message ?? "";
            alert.InnerExceptionMessage = ex?.InnerException?.Message ?? "";
            alert.StackTrace = ex?.StackTrace ?? "";
            alert.DetectionTimeStamp = DateTime.Now;
            alert.Exception = ex;
            AlertList.Add(alert);
        }

        public int Count()
        {
            return AlertList.Count();
        }
    }
}
