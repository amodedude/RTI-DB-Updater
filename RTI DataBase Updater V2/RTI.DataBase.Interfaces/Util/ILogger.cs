using System;
using System.Runtime.CompilerServices;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Objects;
using RTI.DataBase.Util;
using RTI.DataBase.Interfaces.Util;

namespace RTI.DataBase.Interfaces.Util
{
    public interface ILogger : IExceptionProcessor
    {
        string LogFileFullPath { get; }
        string LogFolder { get; }
        EmailAlertList AlertList { get; }
        void WriteMessageToLog(string message, Priority priority = Priority.Info, params object[] args);
        void WriteErrorToLog(Exception ex, string message = "", bool addToEmail = false, Priority priority = Priority.Error, [CallerMemberName]string method = "", [CallerLineNumber] int line = -999, [CallerFilePath] string ClassPath = "");
        int Count();
    }
}
