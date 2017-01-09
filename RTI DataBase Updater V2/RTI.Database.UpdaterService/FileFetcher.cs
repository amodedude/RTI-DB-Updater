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

namespace RTI.DataBase.UpdaterService
{
    internal class FileFetcher
    {
        internal FileFetcher()
        {
            _currentFolder = Path.Combine(Application.Settings.DownloadRepository, DateTime.Now.ToString("MMddyyyHHmmss"));
        }

        public string CurrentFolder { get { return _currentFolder; } private set { } }
        private string _currentFolder = string.Empty;
        private int _filesDownloaded = 0;
        private int _numberOfFilesToDownload = 0;
        private HashSet<source> _initializedDownloads = new HashSet<source>();
        private HashSet<source> _failedDownloads = new HashSet<source>();

        /// <summary>
        /// Download 
        /// USGS text files asynchronously. 
        /// </summary>
        internal HashSet<source> fetchFiles()
        {
            try
            {
                // Get the list of sources from the RTI database
                HashSet<source> goodFiles = new HashSet<source>();
                Logger.WriteToLog("Fetching the list of sources from the RTI database.");
                RTIDBModel RTIContext = new RTIDBModel();
                List<source> sourceList = RTIContext.sources.ToList();
                _numberOfFilesToDownload = sourceList.Count() - 1;

                if (!Application.Settings.UseLatestCachedFiles)
                {
                    // Begin Asynchronous downloading from the USGS
                    Parallel.ForEach(sourceList, new ParallelOptions { MaxDegreeOfParallelism = Application.Settings.MaxDegreeOfParallelism }, source =>
                    {
                        InitilizeDownload(source);
                    });

                    // Validate that file was downloaded
                    foreach (source goodFile in _initializedDownloads)
                        if (!File.Exists(Path.Combine(_currentFolder, goodFile.agency_id + ".txt")))
                            _failedDownloads.Add(goodFile);

                    // Log failed downloads
                    foreach (source badFile in _failedDownloads)
                        Logger.WriteToLog($"Unable to download file with USGSID = {badFile.agency_id:N}, Name = {badFile.full_site_name}", Priority.Error);

                    Logger.WriteToLog($"\r\nFile download(s) complete @{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")}\r\nInitializing upload process...");
                    goodFiles = new HashSet<source>(_initializedDownloads.Where(r => !_failedDownloads.Contains(r)).ToArray());
                }
                else
                {
                    Logger.WriteToLog("Using Latest Cached files, no files to download.");
                    DirectoryInfo latest = new DirectoryInfo(Application.Settings.DownloadRepository).GetDirectories()
                       .OrderByDescending(d => d.LastWriteTimeUtc).First();
                    if (latest != null && latest.Exists)
                    {
                        Logger.WriteToLog($"Cache directory = {latest.FullName}");
                        _currentFolder = latest.FullName;
                        foreach (string file in Directory.EnumerateFiles(latest.FullName))
                        {
                            source source = sourceList.Where(s => s.agency_id == Path.GetFileNameWithoutExtension(file)).FirstOrDefault();
                            if (source != null && !goodFiles.Contains(source))
                                goodFiles.Add(source);
                        }
                    }
                }

                return goodFiles;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorToLog(ex, "A Fatal Error has occurred while fetching USGS source files.", true); 
                throw ex;
            }
        }

        private void InitilizeDownload(source source)
        {
            // Get the USGSID
            try
            {
                string USGSID = source.agency_id;
                if (!_initializedDownloads.Contains(source))
                {
                    string file_name = USGSID + ".txt";                        

                    if (!Directory.Exists(_currentFolder))
                        Directory.CreateDirectory(_currentFolder);

                    string filePath = Path.Combine(_currentFolder, file_name);
                    download_file(USGSID, filePath); // Fetch the file
                                                     //parseFile.ReadFile(filePath, USGSID); // Read the fetched file contents 
                    _initializedDownloads.Add(source);
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Break();
                Logger.WriteToLog($"\r\nError: Unable to download file {_filesDownloaded + 1} of {_numberOfFilesToDownload}.\r\nSite ID = {source.agency_id:N}, \r\nName = {source.full_site_name}");
                Logger.WriteToLog("Error: " + ex.Message + "\r\nInner: " + ((ex?.InnerException == null) ? "\r\n" : ex.InnerException.Message+"\r\n"));
                _failedDownloads.Add(source);
            }
            finally
            {
                _filesDownloaded++;
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
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                using (WebClientWithTimeOut client = new WebClientWithTimeOut() { Timeout = TimeSpan.FromSeconds(Application.Settings.DownloadTimeOutSeconds) })
                {
                    string USGS_URL = "http://nwis.waterdata.usgs.gov/nwis/uv?cb_00095=on&format=rdb&site_no=" + USGSID + "&period=1095";
                    Uri USGS_URI = new Uri(USGS_URL, UriKind.Absolute);
                    client.DownloadFile(USGS_URI, filePath);
                    client.Dispose();
                    Thread.Sleep(3); 
                }
        }
    }
}
