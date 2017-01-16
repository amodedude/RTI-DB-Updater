using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.Download
{
    public class URIBuilder
    {
        /// <summary>
        /// Build the USGS URL
        /// for a given USGS ID input.
        /// </summary>
        /// <param name="usgsid">IDs must be 8-15 digits long</param>
        /// <returns></returns>
        public string BuildUri(string usgsid)
        {
            var parameterList = new List<string>(USGS.Settings.ParameterCodes);
            var paramCodes = $"cd_{string.Join("=1&cd_", (parameterList))}=1";
            var fileFormat = $"format={USGS.Settings.FileFormatSpecifier.Trim()}";
            var siteNumer = $"site_no={usgsid}";
            var uri = USGS.Settings.ApiUri.TrimEnd('/') + '/' + USGS.Settings.OutputDataType.Trim() + '?'
                ;
            uri = string.Join("&", uri + paramCodes, fileFormat, siteNumer);
            return uri;
        }
    }
}
