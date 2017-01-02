using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Util
{
    class WebClientWithTimeOut : WebClient
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
