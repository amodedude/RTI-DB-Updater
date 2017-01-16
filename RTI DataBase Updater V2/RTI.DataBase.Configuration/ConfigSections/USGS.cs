﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RTI.DataBase.Updater.Config
{
    public class USGS : ConfigurationSection
    {
        public static USGS Settings { get; } = ConfigurationManager.GetSection("USGS") as USGS;

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
        
        [ConfigurationProperty("GzipCompression", IsRequired = true)]
        public bool GzipCompression
        {
            get { return (bool)this["GzipCompression"]; }
        }
    }
}