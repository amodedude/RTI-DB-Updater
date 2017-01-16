using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Schedule : ConfigurationSection
    {
        public static Schedule Settings { get; } = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("Schedule") as Schedule;

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
