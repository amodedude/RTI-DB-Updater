using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RTI.DataBase.UpdaterService.Download;
using RTI.DataBase.UpdaterService.Parse;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

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
                TextFileDownloader downloader = new TextFileDownloader();
                string currentFolder = null;

                if (Application.Settings.PerformWaterConductivityUpdate)
                {
                    LogWriter.WriteMessageToLog("Performing Water Conductivity Data DB update...");
                    // Download USGS water data
                    WaterDataFileFetcher fetcher = new WaterDataFileFetcher(LogWriter, downloader);
                    SourceCollection sources = fetcher.FetchFiles();


                    // Upload water data to RTI DataBase
                    LogWriter.WriteMessageToLog("Initializing water data upload process...");
                    UploadFiles(sources, fetcher);
                    currentFolder = fetcher.CurrentFolder;
                }

                if (Application.Settings.PerformWaterSourcesListUpdate)
                {
                    LogWriter.WriteMessageToLog("Performing Sources List DB update...");
                    // Download USGS sources data
                    SourcesFileFetcher sourcesFetcher = new SourcesFileFetcher(LogWriter, downloader,
                        currentFolder);
                    sourcesFetcher.FetchFiles();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteErrorToLog(ex, "The Update Process has encountered a fatal error.", true);
            }
            finally
            {
                SendEmails();   
            }
        }

        /// <summary>
        /// Parse and Upload 
        /// data for each water 
        /// source.
        /// </summary>
        private void UploadFiles(SourceCollection sources, WaterDataFileFetcher fetcher)
        {
            LogWriter.WriteMessageToLog("Initiating File upload process...\r\n");
            WaterDataFileParser parser = new WaterDataFileParser(LogWriter);
            foreach(source source in sources)
            {
                string path = Path.Combine(fetcher.CurrentFolder, source.agency_id+".txt");
                if(File.Exists(path))
                    parser.ReadFile(path, source.agency_id);
            }
        }

        private void SendEmails()
        {
            try
            {
                Emailer.FireEmailAlerts();
            }
            catch (Exception ex)
            {
                LogWriter.WriteErrorToLog(ex, "Unable to send emails.");
            }
        }
    }
}