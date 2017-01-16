using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;

namespace RTI.Database.UpdaterService.Download
{
    public class TextFileDownloader : IDownloader
    {
        private ILogger LogWriter;

        public TextFileDownloader(ILogger logger)
        {
            LogWriter = logger;
        }

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
            LogWriter.WriteMessageToLog("Downloading File from " + uri);
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            using (
                var client = new WebClientWithTimeOut()
                {
                    Timeout = TimeSpan.FromSeconds(Application.Settings.DownloadTimeOutSeconds),
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
