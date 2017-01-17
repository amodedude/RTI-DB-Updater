using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.Download
{
    public class FileFetcher
    {
        public FileFetcher(ILogger logger, IDownloader downloader)
        {
            LogWriter = logger;
            Downloader = downloader;
            string fileRepo = Application.Settings?.DownloadRepository ??
                              (Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "RTI File Repository");
            _currentFolder = Path.Combine(fileRepo, DateTime.Now.ToString("MMddyyyHHmmss"));
        }

        private readonly ILogger LogWriter;
        private readonly IDownloader Downloader;
        public string CurrentFolder { get { return _currentFolder; } private set { } }
        private string _currentFolder = string.Empty;
        private int _filesDownloaded = 0;
        private int _numberOfFilesToDownload = 0;
        private static HashSet<source> _initializedDownloads = new HashSet<source>();
        private static HashSet<source> _failedDownloads = new HashSet<source>();

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
                LogWriter.WriteMessageToLog("Fetching the list of sources from the RTI database.");

                // Get a list of all water sources
                List<source> sourceList;
                using (UnitOfWork uoa = new UnitOfWork())
                    sourceList = uoa.Sources.GetAllSources().ToList();

                _numberOfFilesToDownload = sourceList.Count() - 1;

                if (!Application.Settings.UseLatestCachedFiles)
                {
                    // Begin Asynchronous downloading from the USGS
                    var loopOptions = new ParallelOptions { MaxDegreeOfParallelism = Application.Settings.MaxDegreeOfParallelism };
                    Parallel.ForEach(sourceList, loopOptions, InitilizeDownload);

                    ValidateDownloadedFiles();

                    LogWriter.WriteMessageToLog($"\r\nFile download(s) complete @{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")}\r\nInitializing upload process...");
                    goodFiles = new HashSet<source>(_initializedDownloads.Where(r => !_failedDownloads.Contains(r)).ToArray());
                }
                else
                {
                    LogWriter.WriteMessageToLog("Using Latest Cached files, no files to download.");
                    DirectoryInfo latest = new DirectoryInfo(Application.Settings.DownloadRepository).GetDirectories()
                       .OrderByDescending(d => d.LastWriteTimeUtc).First();
                    if (latest != null && latest.Exists)
                    {
                        LogWriter.WriteMessageToLog($"Cache directory = {latest.FullName}");
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
                LogWriter.WriteErrorToLog(ex, "A Fatal Error has occurred while fetching USGS source files.", true); 
                throw ex;
            }
        }

        /// <summary>
        /// Validate that all USGS sources 
        /// have been fetched to file 
        /// and log any missing output.
        /// </summary>
        private void ValidateDownloadedFiles()
        {
            // Validate that file was downloaded
            foreach (source goodFile in _initializedDownloads)
                if (!File.Exists(Path.Combine(_currentFolder, goodFile.agency_id + ".txt")))
                    _failedDownloads.Add(goodFile);

            // Log failed downloads
            foreach (source badFile in _failedDownloads)
                LogWriter.WriteMessageToLog(
                    $"Unable to download file with USGSID = {badFile.agency_id:N}, Name = {badFile.full_site_name}",
                    Priority.Error);
        }


        /// <summary>
        /// Begin water data download
        /// for a given source.
        /// </summary>
        /// <param name="source"></param>
        private void InitilizeDownload(source source)
        {
            // Return if another thread has already initiated the download.
            if (_initializedDownloads.Contains(source)|| source == null)
                return;

            // Get the USGSID
            string uri = "";
            try
            {
                var usgsid = source.agency_id;
                var fileName = usgsid + ".txt";                        

                if (!Directory.Exists(_currentFolder))
                    Directory.CreateDirectory(_currentFolder);

                var filePath = Path.Combine(_currentFolder, fileName);
                var builder = new URIBuilder();
                uri = builder.BuildUri(usgsid);
                LogWriter.WriteMessageToLog("Downloading File from " + uri);
                Downloader.download_file(uri, filePath, USGS.Settings.GzipCompression); // Fetch the file
                _initializedDownloads.Add(source);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Break();
                LogWriter.WriteMessageToLog($"\r\nError: Unable to download file {_filesDownloaded + 1} of {_numberOfFilesToDownload}.\r\nSite ID = {source.agency_id:N}\r\nName = {source.full_site_name}\r\nURI = {uri}");
                LogWriter.WriteMessageToLog("Error: " + ex.Message + "\r\nInner: " + ((ex?.InnerException == null) ? "\r\n" : ex.InnerException.Message+"\r\n"));
                _failedDownloads.Add(source);
            }
            finally
            {
                _filesDownloaded++;
            }
        }
    }
}
