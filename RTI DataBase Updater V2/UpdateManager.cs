using RTI.DataBase.Model;
using RTI.DataBase.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RTI.DataBase.Updater
{
    internal class UpdateManager
    {
        internal void RunUpdate()
        {
            try
            {
                Logger.WriteToLog("Performing DB update...");

                // Download all USGS Sources
                FileFetcher fetcher = new FileFetcher();
                HashSet<source> sources = fetcher.fetchFiles();

                // Upload to RTI DataBase
                UploadFiles(sources, fetcher);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorToLog(ex, "The Update Process has encountered a fatal error.", true);
            }
            finally
            {
                Emailer.FireEmailAlerts();
            }
        }

        /// <summary>
        /// Parse and Upload 
        /// data for each water 
        /// source.
        /// </summary>
        private void UploadFiles(HashSet<source> sources, FileFetcher fetcher)
        {
            FileParser parser = new FileParser();
            foreach(source source in sources)
            {
                string path = Path.Combine(fetcher.CurrentFolder, source.agency_id+".txt");
                if(File.Exists(path))
                    parser.ReadFile(path, source.agency_id);
            }
        }
    }
}