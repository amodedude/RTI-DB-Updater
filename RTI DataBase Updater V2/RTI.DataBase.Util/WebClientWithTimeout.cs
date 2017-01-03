using System;
using System.Net;

namespace RTI.DataBase.Util
{
    public class WebClientWithTimeOut : WebClient
    {
        public TimeSpan? Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            if (this.Timeout.HasValue)
            {
                webRequest.Timeout = (int)this.Timeout.Value.TotalMilliseconds;
            }
            return webRequest;
        }
    }
}
