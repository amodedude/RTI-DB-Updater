using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.API
{
    public class GeoCodeURIBuilder : IUriBuilder
    {
        private readonly string _lat;
        private readonly string _lng;
        public GeoCodeURIBuilder(string lat, string lng)
        {
            _lat = lat;
            _lng = lng;
        }

        public string BuildUri(string param = null)
        {
            string uri = GeoCodeApi.Settings.ApiUri;
            string format = GeoCodeApi.Settings.Format;
            string zoom = GeoCodeApi.Settings.Zoom.ToString();
            string requestPath = string.Join("&", uri, "lat="+_lat, "lon="+_lng, "format="+format, "zoom="+zoom, "adressdetails=1");

            return requestPath;
        }
    }
}
