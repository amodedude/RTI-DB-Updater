using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Security : ConfigurationSection
    {
        private static Security _settings = ConfigurationManager.GetSection("Security") as Security;

        public static Security Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("EncryptConnectionStrings", IsRequired = true)]
        public bool EncryptConnectionStrings
        {
            get { return (bool)this["EncryptConnectionStrings"]; }
        }

        [ConfigurationProperty("EncyptionKey", IsRequired = true)]
        public string EncyptionKey
        {
            get { return (string)this["EncyptionKey"]; }
        }
    }
}
