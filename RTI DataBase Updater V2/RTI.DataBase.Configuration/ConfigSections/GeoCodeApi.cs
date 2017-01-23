using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RTI.DataBase.Updater.Config
{
    public class GeoCodeApi : ConfigurationSection
    {
        public static GeoCodeApi Settings { get; } = ConfigurationManager.GetSection("GeoCodeApi") as GeoCodeApi;

        [ConfigurationProperty("ApiURI", IsRequired = true)]
        public string ApiUri
        {
            get { return (string)this["ApiURI"]; }
        }

        [ConfigurationProperty("Format", IsRequired = true)]
        public string Format
        {
            get { return (string)this["Format"]; }
        }

        [ConfigurationProperty("Zoom", IsRequired = true)]
        public int Zoom
        {
            get { return (int)this["Zoom"]; }
        }

        [ConfigurationProperty("MaxReqRateSeconds", IsRequired = true)]
        public int MaxReqRateSeconds
        {
            get { return (int)this["MaxReqRateSeconds"]; }
        }
    }
}

