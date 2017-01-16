using System;
using System.Net;

namespace RTI.DataBase.Util
{
    public class WebClientWithTimeOut : WebClient
    {
        public TimeSpan? Timeout { get; set; }
        public string Header { get; set; }
        public bool GzipCompression { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest webRequest = base.GetWebRequest(uri) as HttpWebRequest;
            if (GzipCompression)
            {
                webRequest.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,compress";
                webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            if (!this.Timeout.HasValue)
                return webRequest;

            webRequest.Timeout = (int)this.Timeout.Value.TotalMilliseconds;
            return webRequest;
        }
    }
}
