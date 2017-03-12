using System.Collections.Generic;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.API
{
    public class WaterDataURIBuilder : IUriBuilder
    {
        /// <summary>
        /// Build the USGS URL
        /// for a given USGS ID input.
        /// </summary>
        /// <param name="usgsid">IDs must be 8-15 digits long</param>
        /// <returns></returns>
        public string BuildUri(string usgsid)
        {
            var parameterList = new List<string>(UsgsApi.Settings.ParameterCodes);
            var paramCodes = $"cd_{string.Join("=1&cd_", (parameterList))}=1";
            var fileFormat = $"format={UsgsApi.Settings.FileFormatSpecifier.Trim()}";
            var siteNumer = $"site_no={usgsid}";
            var uri = UsgsApi.Settings.ApiUri.TrimEnd('/') + '/' + UsgsApi.Settings.OutputDataType.Trim() + '?'
                ;
            uri = string.Join("&", uri + paramCodes, fileFormat, siteNumer);
            return uri;
        }
    }
}
