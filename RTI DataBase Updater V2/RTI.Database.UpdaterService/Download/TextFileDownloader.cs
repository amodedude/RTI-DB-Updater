using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;

namespace RTI.Database.UpdaterService.Download
{
    public class TextFileDownloader : IFileDownloader
    {
        /// <summary>
        /// Downloads the USGS text files 
        /// containing conductivity information. 
        /// </summary>
        /// <param name="usgsid"></param>
        /// <param name="filePath"></param>
        /// <returns>
        /// Returns the downloaded files full path.
        /// </returns>
        public void download_file(string uri, string filePath, bool useCompression)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            using (
                var client = new WebClientWithTimeOut()
                {
                    Timeout = TimeSpan.FromMilliseconds(Application.Settings.DownloadTimeOutMilliseconds),
                    GzipCompression = useCompression
                })
            {
                var usgsUri = new Uri(uri, UriKind.Absolute);
                client.DownloadFile(usgsUri, filePath);
                client.Dispose();
                Thread.Sleep(3);
            }
        }
    }
}
