using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using RTI.DataBase.Util;
using RTI.DataBase.Model;

namespace RTI.DataBase.Updater
{
    class FileFetcher
    {
        private bool paused;
        public bool download_finished = true;
        string temp;

        /// <summary>
        /// Creates an new thread to download 
        /// USGS text files asynchronously. 
        /// </summary>
        public void fetchFiles()
        {
            List<string> failedSiteIDs = new List<string>();
            try
            {
                // Get the list of sources from the RTI database
                Logger.WriteToLog("Fetching the list of sources from the RTI database.");
                RTIDBModel RTIContext = new RTIDBModel();
                var sourceList = RTIContext.sources.ToList();
                int numberOfFilesToDownload = sourceList.Count() - 1;
                int filesDownloaded = 0;

                // Begin downloading from the USGS
                while (filesDownloaded < numberOfFilesToDownload) // Cancel if requested
                {
                    Parallel.ForEach(sourceList, async source =>
                    {
                        // Get the USGSID
                        try
                        {
                            string USGSID = source.agency_id;
                            string file_name = USGSID + ".txt";
                            string folder_path = @"C:\Users\John\Desktop\RTI File Repository\";
                            string filePath = folder_path + file_name;
                            await download_file(USGSID, filePath); // Fetch the file
                            //parseFile.ReadFile(filePath, USGSID); // Read the fetched file contents 
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteToLog("Error: " + ex.Message + " Inner" + ex.InnerException);
                            System.Diagnostics.Debugger.Break();
                            Logger.WriteToLog($"\nError: Unable to download file {filesDownloaded + 1} of {numberOfFilesToDownload}.\n\nSite ID = {source.agency_id:N}, \nName = {source.full_site_name}");
                            failedSiteIDs.Add(source.agency_id);
                        }
                        finally
                        {
                            filesDownloaded++;
                        }
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
                }

                Logger.WriteToLog("\nFile download(s) complete!\n\nInitializing upload process...");
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Error: " + ex.Message + " Inner" + ex.InnerException);
                //EmailService emailService = new EmailService();
                //List<string> address = new List<string>();
                //address.Add("amodedude@gmail.com");
                //emailService.SendMail(address, "RTI Alert: Error in Database Updater Application", "An Error has occured in the Database Updater Application FileFetcher. \n\nError: \n\n" + ex.ToString());
                throw ex;
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
        private async Task download_file(string USGSID, string filePath)
        {
            Logger.WriteToLog("\nDownloading File with USGSID =  " + Convert.ToString(USGSID));
            Logger.WriteToLog("Downloading File with USGSID =  " + Convert.ToString(USGSID));
            download_finished = false;
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                string USGS_URL = "http://nwis.waterdata.usgs.gov/nwis/uv?cb_00095=on&format=rdb&site_no=" + USGSID + "&period=1095";
                Uri USGS_URI = new Uri(USGS_URL, UriKind.Absolute);
                await client.DownloadFileTaskAsync(USGS_URI, filePath);
                counter = 0;
            }
            download_finished = true;
        }

        private int counter { get; set; }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            counter++;
            //// Prevents this event from triggering so quickly that the console window
            //// glitches and does not have time to clear before printing the next message. 
            //if (counter % 15 == 0)
            //{
            //    if (counter == 15)
            //        temp = UserInterface.GetCurrentOutput();
            //    else
            //        temp = UserInterface.GetCurrentOutput(1);

            //    var KB = (e.BytesReceived / 1000);
            //    UserInterface.ClearUI();
            //    UserInterface.WriteToConsole(temp + "\nDownloaded (kB): " + KB.ToString());
            //}
        }
    }
}
