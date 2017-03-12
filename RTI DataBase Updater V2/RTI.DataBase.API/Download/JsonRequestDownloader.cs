using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Objects.Json;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.API.Download
{
    public class JsonRequestDownloader
    {
        /// <summary>
        /// Http Request default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userAgent">Specifies the caller identification.</param>
        public JsonRequestDownloader(ILogger logger,string userAgent = null)
        {
            Logger = logger;
            User = userAgent ?? Application.Settings.ApiRequestUserAgent;
        }

        private ILogger Logger;
        private string User;

        /// <summary>
        /// Make an HTTP request.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string make_request(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.UserAgent = User;
                WebResponse resp = request.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (Exception ex)
            {
                Logger.WriteMessageToLog($"Unable to make HttpRequest to {uri}");
                Logger.WriteErrorToLog(ex);
                return null;
            }
        }

        /// <summary>
        /// Make a HttpRequest and return 
        /// the JSON response as a KVP.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public GeoCode DownloadJsonResponce(string uri)
        {
            string json = make_request(uri);
            GeoCode values = JsonConvert.DeserializeObject<GeoCode>(json);
            return values;
        }
    }
}
