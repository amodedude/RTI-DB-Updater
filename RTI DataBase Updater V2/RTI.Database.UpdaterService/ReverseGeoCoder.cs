using System;
using System.Collections.Generic;
using System.Threading;
using RTI.Database.UpdaterService.API;
using RTI.Database.UpdaterService.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Objects.Json;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService
{
    public class ReverseGeoCoder
    {
        public ReverseGeoCoder(ILogger logger)
        {
            Logger = logger;
        }

        private ILogger Logger;

        /// <summary>
        /// Appends Geocode data to a 
        /// source using Lat/Lng.
        /// </summary>
        /// <returns></returns>
        public SourceCollection AppendGeoCodeData(SourceCollection sources)
        {
            List<source> updatedList = new List<source>();
            foreach (source src in sources)
            {
                updatedList.Add(AddGeoCode(src));
                Thread.Sleep(TimeSpan.FromSeconds(GeoCodeApi.Settings.MaxReqRateSeconds)); // Adhere to API usage policy.
            }
            return new SourceCollection(updatedList);
        }

        /// <summary>
        /// Add GeoCode data
        /// to a single source.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private source AddGeoCode(source src)
        {
            string lat = src.exact_lat;
            string lng = src.exact_lng;

            var geoCodeData = GetGeoReverseGeocodeData(lat, lng);

            if (geoCodeData != null)
            {
                var adress = geoCodeData.Address;
                src.city = adress.City;
                src.state = adress.State;
                src.county_name = adress.County;
                src.country = adress.Country;
                src.county_number = adress.CountryCode;
                // ToDo: Add Post Code data.
            }

            return src;
        }

        private GeoCode GetGeoReverseGeocodeData(string lat, string lng)
        {
            JsonRequestDownloader downloader = new JsonRequestDownloader(Logger, Application.Settings.ApiRequestUserAgent);
            GeoCodeURIBuilder builder = new GeoCodeURIBuilder(lat, lng);
            string uri = builder.BuildUri();
            var geoCodeData =  downloader.DownloadJsonResponce(uri);
            return geoCodeData;
        }
    }
}
