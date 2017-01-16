using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Security : ConfigurationSection
    {
        public static Security Settings { get; } = ConfigurationManager.GetSection("Security") as Security;

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
