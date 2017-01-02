using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public enum Priority { All, Fatal, Error, Warning, Info };

    public class Log : ConfigurationSection
    {
        private static Log _settings = ConfigurationManager.GetSection("Log") as Log;

        public static Log Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("LogLevel", IsRequired = true)]
        public Priority LogLevel
        {
            get { return (Priority)this["LogLevel"]; }
        }

        [ConfigurationProperty("WriteToFIle", IsRequired = true)]
        public string WriteToFIle
        {
            get { return (string)this["WriteToFIle"]; }
        }

        [ConfigurationProperty("WriteToDB", IsRequired = true)]
        public string WriteToDB
        {
            get { return (string)this["WriteToDB"]; }
        }

        [ConfigurationProperty("WriteToWindowsEventLog", IsRequired = true)]
        public string WriteToWindowsEventLog
        {
            get { return (string)this["WriteToWindowsEventLog"]; }
        }

    }
}
