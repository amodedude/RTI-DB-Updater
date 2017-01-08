using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Schedule : ConfigurationSection
    {
        private static Schedule _settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("Schedule") as Schedule;

        public static Schedule Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("Mode", IsRequired = true)]
        public string Mode
        {
            get { return (string)this["Mode"]; }
        }

        [ConfigurationProperty("IntervalMinutes", IsRequired = true)]
        public string IntervalMinutes
        {
            get { return (string)this["IntervalMinutes"]; }
        }

        [ConfigurationProperty("ScheduledTime", IsRequired = true)]
        public string ScheduledTime
        {
            get { return (string)this["ScheduledTime"]; }
        }

        [ConfigurationProperty("RunOnce", IsRequired = true)]
        public bool RunOnce
        {
            get { return (bool)this["RunOnce"]; }
        }
    }
}
