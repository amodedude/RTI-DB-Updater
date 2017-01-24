using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.UpdaterService.Parse;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.UpdaterService.Download
{
    public class SourcesFileFetcher : IFileFetcher
    {
        public SourcesFileFetcher(ILogger logger, IFileDownloader downloader, string dir = null)
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

            _sourcesFolder = Path.Combine(_currentFolder, "SourcesData");
            _sourcesFile = Path.Combine(_sourcesFolder, "USGSSourcesList.txt");
        }

        private readonly ILogger LogWriter;
        private readonly IFileDownloader Downloader;

        public string CurrentFolder
        {
            get { return _currentFolder; }
            private set { }
        }

        private readonly string _currentFolder;
        private readonly string _sourcesFolder;
        private readonly string _sourcesFile;

        /// <summary>
        /// Fetch an updated 
        /// list of all USGS
        /// water sources that have 
        /// conductivity data. 
        /// </summary>
        /// <returns></returns>
        public SourceCollection FetchFiles()
        {
            LogWriter.WriteMessageToLog("Beginning download of all USGS water sources list.");

            // Get a list of all current water sources
            SourceCollection sourceList;
            using (UnitOfWork uoa = new UnitOfWork())
                sourceList = new SourceCollection(uoa.Sources.GetAllSources().ToList());

            // Download the new USGS sources file.
            if (DownloadSoucesFile())
                LogWriter.WriteMessageToLog("Download completed successfully.");
            else
            {
                LogWriter.WriteMessageToLog("Unable to download USGS water sources list.", Priority.Warning);
                return null;
            }

            // Read the downloaded file
            SourcesFileParser parser = new SourcesFileParser();
            SourceCollection downladedSources = new SourceCollection(parser.ReadFile(_sourcesFile));
            if(downladedSources.Count > 0)
                downladedSources.RemoveAt(0);
            var newSourcesList = new SourceCollection(downladedSources.Where(s => !sourceList.Contains(s)));

            // Append Reverse Geo-code data
            ReverseGeoCoder geoCoder = new ReverseGeoCoder(LogWriter);
            newSourcesList = new SourceCollection(geoCoder.AppendGeoCodeData(newSourcesList));

            // Add Source Names
            newSourcesList = AppendSourceNames(downladedSources);

            return newSourcesList;
        }

        /// <summary>
        /// Extracts water 
        /// source name (River,Creek, Stream, ext...)
        /// </summary>
        /// <param name="newSourcesList"></param>
        /// <returns></returns>
        private SourceCollection AppendSourceNames(IList<source> sourcesList)
        {
            List<source> resultList = new List<source>();
            foreach (source source in sourcesList)
            {
                string river = source.unique_site_name;
                int index = -1;
                List<string> delimList = new List<string>() {" NEAR ", " AT ", "@"," ABOVE "," BELOW "," NR ", "., "} ;
                foreach (var delim in delimList)
                {
                    var result = source.unique_site_name.ToLower().IndexOf(delim.ToLower());
                    if (result != null && result > 0)
                    {
                        index = result;
                        river = source.unique_site_name.Substring(0, index);
                        break;
                    }
                }
                source sourceWithRiver = source;
                sourceWithRiver.river = river;
                resultList.Add(sourceWithRiver);
            }

            //DEBUG
            //foreach (var source in resultList)
            //{
            //    Console.WriteLine(source.river);
            //}

            return  new SourceCollection(resultList);
        }


        /// <summary>
        /// Download the USGS 
        /// sources file.
        /// </summary>
        /// <returns></returns>
        private bool DownloadSoucesFile()
        {
            SourcesURIBuilder builder = new SourcesURIBuilder();
            string uri = builder.BuildUri();

            Directory.CreateDirectory(_sourcesFolder);
            FileInfo info;
            using (var stream = File.CreateText(_sourcesFile))
            {
                stream.Close();
                Downloader.download_file(uri, _sourcesFile, false);
                info = new FileInfo(_sourcesFile);
            }
            return info.Length > 0;
        }
    }
}
