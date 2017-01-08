using System.Linq;
using System.Text;
using System.Web.Security;
using System.Configuration;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.Util
{
    /// <summary>
    /// Cryptographic services 
    /// class used for encrypting
    /// and decrypting sensitive
    /// string data. 
    /// </summary>
    internal class Crypto
    {
        /// <summary>
        /// Returns an appSettings 
        /// configuration section from the 
        /// applications exe.config file. If 
        /// the section is not encrypted, it 
        /// will be encrypted.
        /// </summary>
        /// <param name="exeConfigName"></param>
        /// <returns></returns>
        internal static AppSettingsSection GetEncryptedAppSettingsSection(string exeConfigName)
        {
            // Open the configuration file and retrieve 
            // the connectionStrings section.
            System.Configuration.Configuration config = ConfigurationManager.
                OpenExeConfiguration(exeConfigName);

            AppSettingsSection section =
                    config.GetSection("appSettings")
                    as AppSettingsSection;

            EncryptConfigSection(config, section);
            return section;
        }

        /// <summary>
        /// Returns a connectionStrings 
        /// configuration section from the 
        /// applications exe.config file. If 
        /// the section is not encrypted, it 
        /// will be encrypted.
        /// </summary>
        /// <param name="exeConfigName"></param>
        /// <returns></returns>
        internal static ConnectionStringsSection GetEncryptedConnectionStringsSection(string exeConfigName)
        {
            // Open the configuration file and retrieve 
            // the connectionStrings section.
            System.Configuration.Configuration config = ConfigurationManager.
                OpenExeConfiguration(exeConfigName);

            ConnectionStringsSection section =
                    config.GetSection("connectionStrings")
                    as ConnectionStringsSection;
            
            EncryptConfigSection(config, section);
            return section;
        }

        /// <summary>
        /// Encrypts/Decrypts
       ///  a configuration section
        /// in an exe.config file.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="section"></param>
        internal static void EncryptConfigSection(System.Configuration.Configuration config, ConfigurationSection section)
        {
            //Ensure configuration sections are always encrypted
            if (!section.SectionInformation.IsProtected && Security.Settings.EncryptConnectionStrings)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                config.Save();
            }
            else
            {
                if (Security.Settings.EncyptionKey == "btS8GTXTk7N2xv2xjdBdJ8980pr9ejg7CBVDC5z5LgdVCxuyztMbtw3ramT3KpwZp" && !Security.Settings.EncryptConnectionStrings)
                    section.SectionInformation.UnprotectSection();
            }
        }
    }
}

