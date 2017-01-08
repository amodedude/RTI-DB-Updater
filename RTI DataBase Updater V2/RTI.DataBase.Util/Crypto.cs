using System.Linq;
using System.Text;
using System.Web.Security;
using System.Configuration;

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
        /// Encrypts a configuration section
        /// in an exe.config file.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="section"></param>
        internal static void EncryptConfigSection(System.Configuration.Configuration config, ConfigurationSection section)
        {
            //Ensure config sections are always encrypted
            if (!section.SectionInformation.IsProtected)
            {
                // Encrypt the section.
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                // Save the current configuration.
                config.Save();
            }
        }

        /// <summary>
        /// Encrypts a string using the 
        /// System.Web.MachineKey.Protect
        /// class.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="purpose"></param>
        /// <returns>Returns an encrypted byte array.</returns>
        internal static byte[] Protect(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            byte[] stream = Encoding.UTF8.GetBytes(text);
            byte[] encodedValue = MachineKey.Protect(stream, key);
            return encodedValue;
        }

        /// <summary>
        /// This method decrypts an 
        /// encrypted byte array using 
        /// the specified encryption key.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="purpose"></param>
        /// <returns></returns>
        internal static string Unprotect(byte[] stream, string key)
        {
            if (stream.Count() <= 0)
                return null;

            byte[] decodedValue = MachineKey.Unprotect(stream, key);
            return Encoding.UTF8.GetString(decodedValue);
        }
    }
}

