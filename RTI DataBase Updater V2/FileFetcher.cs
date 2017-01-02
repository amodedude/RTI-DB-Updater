using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using RTI.DataBase.Model;
using System.IO;

namespace RTI.DataBase.Updater
{
    class FileFetcher
    {
        public bool download_finished = true;
        int filesDownloaded = 0;
        int numberOfFilesToDownload = 0;
        List<string> failedSiteIDs = new List<string>();
        HashSet<string> initializedDownloads = new HashSet<string>(); 

        
        /// <summary>
        /// Download 
        /// USGS text files asynchronously. 
        /// </summary>
        public void fetchFiles()
        {
            try
            {
                // Get the list of sources from the RTI database
                Logger.WriteToLog("Fetching the list of sources from the RTI database.");
                RTIDBModel RTIContext = new RTIDBModel();
                var sourceList = RTIContext.sources.ToList();
                int numberOfFilesToDownload = sourceList.Count() - 1;
                 
                // Begin Asynchronous downloading from the USGS
                Parallel.ForEach(sourceList, new ParallelOptions { MaxDegreeOfParallelism = Application.Settings.MaxDegreeOfParallelism }, source =>
                {
                    InitilizeDownload(source);
                });


                //foreach (var source in sourceList) // Loop through each USGS source 
                //{
                //    if (download_finished)
                //    {
                //        double percentage = ((double)filesDownloaded / numberOfFilesToDownload);
                //        UserInterface.WriteToConsole("Total Progress:                                   {0:P}" +
                //            "\n--------------------------------------------------------" +
                //            "\nDownloaded {1} file(s) out of {2}", percentage, filesDownloaded, numberOfFilesToDownload);

                //        // Get the USGSID
                //        try
                //        {
                //            string USGSID = source.agency_id;
                //            string file_name = USGSID + ".txt";
                //            string folder_path = @"C:\Users\John\Desktop\RTI File Repository\";
                //            string filePath = folder_path + file_name;
                //            await download_file(USGSID, filePath); // Fetch the file
                //            parseFile.ReadFile(filePath, USGSID); // Read the fetched file contents 
                //        }
                //        catch (Exception ex)
                //        {
                //            ApplicationLog.WriteMessageToLog("Error: " + ex.Message + " Inner" + ex.InnerException, true, true, true);
                //            System.Diagnostics.Debugger.Break();
                //            UserInterface.WriteToConsole("\nError: Unable to download file {0} of {1}.\n\nSite ID = {2:N}, \nName = {3}",
                //                                          filesDownloaded + 1, numberOfFilesToDownload, source.agency_id, source.full_site_name);
                //            failedSiteIDs.Add(source.agency_id);
                //        }
                //        finally
                //        {
                //            filesDownloaded++;
                //        }
                //    }
                //}


                Logger.WriteToLog("\nFile download(s) complete!\n\nInitializing upload process...");
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Error: " + ex.Message + " Inner" + ex.InnerException);
                //EmailService emailService = new EmailService();
                //List<string> address = new List<string>();
                //address.Add("amodedude@gmail.com");
                //emailService.SendMail(address, "RTI Alert: Error in Database Updater Application", "An Error has occurred in the Database Updater Application FileFetcher. \n\nError: \n\n" + ex.ToString());
                throw ex;
            }
        }

        private void InitilizeDownload(source source)
        {
            // Get the USGSID
            try
            {
                string USGSID = source.agency_id;
                if (!initializedDownloads.Contains(USGSID))
                {
                    string file_name = USGSID + ".txt";
                    string folder_path = Application.Settings.DownloadRepository;
                    string filePath = Path.Combine(folder_path, file_name);
                    download_file(USGSID, filePath); // Fetch the file
                                                     //parseFile.ReadFile(filePath, USGSID); // Read the fetched file contents 
                    initializedDownloads.Add(USGSID);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Break();
                Logger.WriteToLog($"\r\nError: Unable to download file {filesDownloaded + 1} of {numberOfFilesToDownload}.\r\nSite ID = {source.agency_id:N}, \r\nName = {source.full_site_name}");
                Logger.WriteToLog("Error: " + ex.Message + "\r\nInner: " + ((ex?.InnerException == null) ? "" : ex.InnerException.Message));
                failedSiteIDs.Add(source.agency_id);
            }
            finally
            {
                filesDownloaded++;
            }
        }


        /// <summary>
        /// Downloads the USGS text files 
        /// containing conductivity information. 
        /// </summary>
        /// <param name="USGSID"></param>
        /// <returns>
        /// Returns the downloaded files full path.
        /// </returns>
        private void download_file(string USGSID, string filePath)
        {
            Logger.WriteToLog("Downloading File with USGSID =  " + Convert.ToString(USGSID));
            download_finished = false;
            using (var client = new WebClient())
            {
                string USGS_URL = "http://nwis.waterdata.usgs.gov/nwis/uv?cb_00095=on&format=rdb&site_no=" + USGSID + "&period=1095";
                Uri USGS_URI = new Uri(USGS_URL, UriKind.Absolute);
                client.DownloadFile(USGS_URI, filePath);
            }
            download_finished = true;
        }


    }
}
