using System;
using System.Net;

namespace RTI.DataBase.Util
{
    public class WebClientWithTimeOut : WebClient
    {
        public TimeSpan? Timeout { get; set; }
        public string Header { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var webRequest = base.GetWebRequest(uri);
            if (!this.Timeout.HasValue) return webRequest;

            webRequest.Timeout = (int)this.Timeout.Value.TotalMilliseconds;
            return webRequest;
        }
    }
}
