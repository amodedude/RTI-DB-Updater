using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.Database.GeoCoder.Objects
{
    /// <summary>
    /// Represents a ZipCodeTabulation 
    /// flat file. Tabulations retrieved from 
    /// https://www.census.gov/geo/maps-data/data/gazetteer2016.html
    /// </summary>
    public class ZipCodeTabulation
    {
        /// <summary>
        /// 	Five digit ZIP Code Tabulation Area Census Code
        /// </summary>
        public string GeoId { get; set; }

        /// <summary>
        /// Land Area (square meters) - Created for statistical purposes only
        /// </summary>
        public decimal ALand { get; set; }

        /// <summary>
        /// Water Area (square meters) - Created for statistical purposes only
        /// </summary>
        public decimal AWater { get; set; }

        /// <summary>
        /// Land Area (square miles) - Created for statistical purposes only
        /// </summary>
        public decimal ALand_Sqmi { get; set; }

        /// <summary>
        /// Water Area (square miles) - Created for statistical purposes only
        /// </summary>
        public decimal AWater_Sqmi { get; set; }

        /// <summary>
        /// Latitude (decimal degrees) First character is blank or "-" denoting North or South latitude respectively
        /// </summary>
        public decimal IntPtLat { get; set; }

        /// <summary>
        /// Longitude (decimal degrees) First character is blank or "-" denoting East or West longitude respectively
        /// </summary>
        public decimal IntPtLong { get; set; }
    }
}
