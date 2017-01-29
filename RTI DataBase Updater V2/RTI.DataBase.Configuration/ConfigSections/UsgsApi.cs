using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RTI.DataBase.Updater.Config
{
    public class UsgsApi : ConfigurationSection
    {
        public static UsgsApi Settings { get; } = ConfigurationManager.GetSection("UsgsApi") as UsgsApi;

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

        public IEnumerable<string> ParameterCodes { get { return _parameterCodes.Split(',').ToList(); } }
        [ConfigurationProperty("ParameterCodes", IsRequired = true)]
        private string _parameterCodes
        {
            get { return (string)this["ParameterCodes"]; }
        }

        [ConfigurationProperty("StatisticCode", IsRequired = true)]
        public string StatisticCode
        {
            get { return (string)this["StatisticCode"]; }
        }

        [ConfigurationProperty("GzipCompression", IsRequired = true)]
        public bool GzipCompression
        {
            get { return (bool)this["GzipCompression"]; }
        }

        [ConfigurationProperty("DateFormat", IsRequired = true)]
        public string DateFormat
        {
            get { return (string)this["DateFormat"]; }
        }

        [ConfigurationProperty("ColumnMappingXrefHasHeader", IsRequired = true)]
        public bool ColumnMappingXrefHasHeader
        {
            get { return (bool)this["ColumnMappingXrefHasHeader"]; }
        }

        [ConfigurationProperty("RiverNameDelimiters", IsRequired = true)]
        private string RiverDelimiters
        {
            get { return (string)this["RiverNameDelimiters"]; }
        }

        public List<string> RiverNameDelimiters { get { return RiverDelimiters.Split(',').Select(r => r.Trim()).ToList(); } }
    }
}
