using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class App : ConfigurationSection
    {
        private static App _settings = ConfigurationManager.GetSection("App") as App;

        public static App Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("Mode", IsRequired = true)]
        public string Mode
        {
            get { return (string)this["Mode"]; }
        }
    }
}
