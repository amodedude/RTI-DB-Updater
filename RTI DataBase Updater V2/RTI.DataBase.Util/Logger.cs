using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.Util
{
    public static class Logger
    {
        static string _logPath;
        static DateTime _date = new DateTime();

        /// <summary>
        /// Initialize a new 
        /// Logging session.
        /// </summary>
        static Logger()
        {
            _date = DateTime.UtcNow;
            _logPath = Application.StartupPath;
        }

        public static void WriteToLog(string message, Priority priority = Priority.Info, params object[] args)
        {
            Priority logLevel = Log.Settings.LogLevel;
            if (priority > logLevel)
            {
                Console.WriteLine(message,args);
            }
        }        
    }
}
