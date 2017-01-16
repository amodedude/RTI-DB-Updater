using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using RTI.DataBase.Interfaces;

namespace RTI.DataBase.UpdaterService
{
    public class UpdateManager
    {
        private IEmailer Emailer;
        private ILogger LogWriter;
        public UpdateManager(ILogger logger, IEmailer emailer)
        {
            Emailer = emailer;
            LogWriter = logger;
        }

        public void RunUpdate()
        {
            try
            {
                LogWriter.WriteMessageToLog("Performing DB update...");

                // Download all USGS Sources
                FileFetcher fetcher = new FileFetcher(LogWriter);
                HashSet<source> sources = fetcher.fetchFiles();

                // Upload to RTI DataBase
                UploadFiles(sources, fetcher);
            }
            catch (Exception ex)
            {
                LogWriter.WriteErrorToLog(ex, "The Update Process has encountered a fatal error.", true);
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
            FileParser parser = new FileParser(LogWriter);
            foreach(source source in sources)
            {
                string path = Path.Combine(fetcher.CurrentFolder, source.agency_id+".txt");
                if(File.Exists(path))
                    parser.ReadFile(path, source.agency_id);
            }
        }
    }
}