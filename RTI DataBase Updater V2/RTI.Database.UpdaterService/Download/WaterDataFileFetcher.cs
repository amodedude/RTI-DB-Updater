using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using RTI.DataBase.API;

namespace RTI.DataBase.UpdaterService.Download
{
    public class WaterDataFileFetcher : IFileFetcher
    {
        public WaterDataFileFetcher(ILogger logger, IFileDownloader downloader, string dir = null)
        {
            LogWriter = logger;
            Downloader = downloader;
            if (dir == null)
            {
                string fileRepo = Application.Settings?.DownloadRepository ??
                                  (Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "RTI File Repository");
                _currentFolder = Path.Combine(fileRepo, DateTime.Now.ToString("MMddyyyHHmmss"));
            }
            else
            {
                _currentFolder = dir;
            }
        }

        private readonly ILogger LogWriter;
        private readonly IFileDownloader Downloader;
        public string CurrentFolder { get { return _currentFolder; } private set { } }
        private string _currentFolder = string.Empty;
        private int _filesDownloaded = 0;
        private int _numberOfFilesToDownload = 0;
        private static SourceCollection _initializedDownloads = new SourceCollection();
        private static SourceCollection _failedDownloads = new SourceCollection();
        private  static object lockObject = new object();

        /// <summary>
        /// Download 
        /// USGS text files asynchronously. 
        /// </summary>
        public SourceCollection FetchFiles()
        {
            try
            {
                // Get the list of sources from the RTI database
                SourceCollection goodFiles = new SourceCollection();
                LogWriter.WriteMessageToLog("Fetching the current list of sources from the RTI database.");

                // Get a list of all water sources
                List<source> sourceList;
                using (UnitOfWork uoa = new UnitOfWork())
                    sourceList = uoa.Sources.GetAllSources().ToList();

                _numberOfFilesToDownload = sourceList.Count() - 1;

                if (!Application.Settings.UseLatestCachedFiles)
                {
                    // Remove and log Duplicates
                    sourceList = LogAndRemoveDuplicates(sourceList);

                    // Begin Asynchronous downloading from the USGS
                    var loopOptions = new ParallelOptions { MaxDegreeOfParallelism = Application.Settings.MaxDegreeOfParallelism };
                    Parallel.ForEach(sourceList, loopOptions, InitilizeDownload);

                    // Validate all downloaded files
                    var message = !ValidateDownloadedFiles() ? 
                        $"\r\nFile download(s) complete with 0 errors @{DateTime.Now:dd/MM/yyyy hh:mm:ss tt}" : 
                        $"\r\nFile download(s) complete with {_failedDownloads.Count} error(s) @{DateTime.Now:dd/MM/yyyy hh:mm:ss tt}";

                    LogWriter.WriteMessageToLog(message);
                    goodFiles = new SourceCollection(_initializedDownloads.Where(r => !_failedDownloads.Contains(r)).ToArray());
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
                            source source = sourceList.FirstOrDefault(s => s.agency_id == Path.GetFileNameWithoutExtension(file));
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
                throw;
            }
        }

        private List<source> LogAndRemoveDuplicates(IEnumerable<source> sourceList)
        {
            LogWriter.WriteMessageToLog("Detecting duplicate sources...");
            var duplicateKeys = sourceList.GroupBy(x => x.agency_id)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key);

            var dupes = sourceList.Where(s => duplicateKeys.Contains(s.agency_id)).ToList();
            var nonDupes = sourceList.Where(s => !duplicateKeys.Contains(s.agency_id)).ToList();

            // Delete all duplicate sources
            using (UnitOfWork uoa = new UnitOfWork())
            {
                List<source> dupeSources = uoa.Sources.GetAllSources().Where(r => duplicateKeys.Contains(r.agency_id)).ToList();
                if (dupeSources != null && dupeSources.Count > 0)
                {
                    foreach (source src in dupeSources)
                    {
                        var linkedCustomerWaters = uoa.CustomerWaters.GetAllCustomerWatersWithSouce(src);
                        if (!(linkedCustomerWaters.Count() > 0))
                        {
                            LogWriter.WriteMessageToLog($"Duplicate value for {src.agency}-{src.agency_id}: {src.full_site_name}");
                            uoa.Sources.Remove(src);
                        }
                        else
                        {
                            LogWriter.WriteErrorToLog(new Exception($"Duplicate value for {src.agency}-{src.agency_id}: {src.full_site_name}, source cannot be removed due to FK constraint."), "Error deleting duplicate source from sources table.", true, Priority.Warning);
                        }
                    }
                    uoa.Complete();

                    // Write any duplicate water sources to an XML file.
                    XMLSerialization serializer = new XMLSerialization();
                    serializer.Serialize(dupes.ToList(), Application.Settings.XmlOutputFolder + "DuplicateWaterSources.xml");
                }
                else
                {
                    LogWriter.WriteMessageToLog("No duplicate water sources found.");
                }
            }

            return nonDupes.ToList();
        }

        /// <summary>
        /// Validate that all USGS sources 
        /// have been fetched to file 
        /// and log any missing output.
        /// </summary>
        private bool ValidateDownloadedFiles()
        {
            bool isValid = true;
            // Validate that file was downloaded
            foreach (source goodFile in _initializedDownloads)
                if (!File.Exists(Path.Combine(_currentFolder, goodFile.agency_id + ".txt")))
                    _failedDownloads.Add(goodFile);

            // Log failed downloads
            foreach (source badFile in _failedDownloads)
                LogWriter.WriteMessageToLog(
                    $"Unable to download file with USGSID = {badFile.agency_id:N}, Name = {badFile.full_site_name}",
                    Priority.Error);

            if (_failedDownloads.Count > 0)
                isValid = false;
            
            return isValid;
        }


        /// <summary>
        /// Begin water data download
        /// for a given source.
        /// </summary>
        /// <param name="source"></param>
        private void InitilizeDownload(source source)
        {
            lock (lockObject)
            {
                // Return if another thread has already initiated the download.
                if (_initializedDownloads.Contains(source) || source == null)
                    return;
            }

            // Get the USGSID
                string uri = "";
                try
                {
                    var usgsid = source.agency_id;
                    var fileName = usgsid + ".txt";

                    if (!Directory.Exists(_currentFolder))
                        Directory.CreateDirectory(_currentFolder);

                    var filePath = Path.Combine(_currentFolder, fileName);
                    var builder = new WaterDataURIBuilder();
                    uri = builder.BuildUri(usgsid);
                    LogWriter.WriteMessageToLog("Downloading File from " + uri);
                    Downloader.download_file(uri, filePath, UsgsApi.Settings.GzipCompression); // Fetch the file
                    lock (lockObject)
                    {
                        _initializedDownloads.Add(source);
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObject)
                    {
                        //System.Diagnostics.Debugger.Break();
                        string message =
                            $"Unable to download file {_filesDownloaded + 1} of {_numberOfFilesToDownload}.\r\nSite ID = {source.agency_id:N}\r\nName = {source.full_site_name}\r\nURI = {uri}";
                        LogWriter.WriteErrorToLog(ex, message, true);
                        _failedDownloads.Add(source);
                    }
                }
                finally
                {
                    Interlocked.Increment(ref _filesDownloaded);
                }
        }
    }
}
