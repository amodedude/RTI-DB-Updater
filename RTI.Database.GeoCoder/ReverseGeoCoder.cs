using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using RTI.DataBase.API;
using RTI.DataBase.API.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Objects.Json;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.GeoCoder
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
                Logger.WriteMessageToLog($"Retrieving GeoCode data for source {src.agency}-{src.agency_id}, {src.unique_site_name}");
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

            var geoCodeData = GetReverseGeocodeData(lat, lng);

            if (geoCodeData != null)
            {
                var Address = geoCodeData.Address;
                src.city = Address.City;
                src.state = States.GetAbbreviation(Address.State) ?? "";
                src.state_name = Address.State;
                src.region = States.GetRegion(Address.State) ?? "";
                src.county_name = Address.County;
                src.country = Address.Country;
                src.post_code = Address.PostCode;
                src.street_number = Address.AddressNumber;
                src.full_site_name = geoCodeData.DisplayName;
                src.street_lat = geoCodeData.Lat;
                src.street_lng = geoCodeData.Lon;
                string milesFromSite = MilesBetweenCoordinates(geoCodeData.Lat, geoCodeData.Lon, src.exact_lat, src.exact_lng).ToString();
                src.miles_from_site = (milesFromSite == "-999" ? "" : milesFromSite);
            }

            return src;
        }

        /// <summary>
        /// Calculate the 
        /// distance between the 
        /// actual site lat/lon and 
        /// the reverse GeoCoded coordinates.
        /// SOURCE: http://andrew.hedges.name/experiments/haversine/
        /// </summary>
        /// <returns></returns>
        public double MilesBetweenCoordinates(string slat1, string slon1 , string slat2, string slon2)
        {
            try
            {
                // Convert to Radians for calculation
                double lat1 = Math.PI * double.Parse(slat1) / 180.0, lon1 = Math.PI * double.Parse(slon1) / 180.0;
                double lat2 = Math.PI * double.Parse(slat2) / 180.0, lon2 = Math.PI * double.Parse(slon2) / 180.0;

                // Calculate distance
                double dLat = lat2 - lat1;
                double dLon = lon2 - lon1;
                double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double d = 3961 * c;
                return Math.Round(d,3);
            }
            catch
            {
                return -999;
            }
        }

        private GeoCode GetReverseGeocodeData(string lat, string lng)
        {
            JsonRequestDownloader downloader = new JsonRequestDownloader(Logger, Application.Settings.ApiRequestUserAgent);
            GeoCodeURIBuilder builder = new GeoCodeURIBuilder(lat, lng);
            string uri = builder.BuildUri(Application.Settings.ApiRequestUserAgent);
            var geoCodeData =  downloader.DownloadJsonResponce(uri + "&email=" + Email.Settings.From);
            return geoCodeData;
        }
    }
}
