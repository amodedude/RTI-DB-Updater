using System.Configuration;

namespace RTI.DataBase.Updater.Config.ConfigSections
{
    public class USGS : ConfigurationSection
    {
        public static USGS Settings { get; } = ConfigurationManager.GetSection("Application") as USGS;

        [ConfigurationProperty("ApiURI", IsRequired = true)]
        public string ApiUri
        {
            get { return (string)this["ApiURI"]; }
        }

        [ConfigurationProperty("OutputDataType", IsRequired = true)]
        public string OutputDataType
        {
            get { return (string)this["OutputDataType"]; }
        }

        [ConfigurationProperty("FileFormatSpecifier", IsRequired = true)]
        public string FileFormatSpecifier
        {
            get { return (string)this["FileFormatSpecifier"]; }
        }

        [ConfigurationProperty("SearchPeriodDays", IsRequired = true)]
        public int SearchPeriodDays
        {
            get { return (int)this["SearchPeriodDays"]; }
        }

        [ConfigurationProperty("ParameterCodes", IsRequired = true)]
        public string ParameterCodes
        {
            get { return (string)this["ParameterCodes"]; }
        }

        [ConfigurationProperty("GzipCompression", IsRequired = true)]
        public bool GzipCompression
        {
            get { return (bool)this["GzipCompression"]; }
        }
    }
}
