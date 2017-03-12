using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RTI.DataBase.API.Parse;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.GeoCoder;
using RTI.DataBase.API;

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

            // Get a list of all current water sources
            LogWriter.WriteMessageToLog("Beginning download of all USGS water sources list.");
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

            // Update the list with ZipCode data


            // Add Source Names
            newSourcesList = AppendSourceNames(newSourcesList);

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
            List<string> delimList = UsgsApi.Settings.RiverNameDelimiters;
            delimList = delimList.Select(r => r.PadLeft(r.Length+1).PadRight(r.Length+2)).ToList();
            List<source> resultList = new List<source>();
            LogWriter.WriteMessageToLog("Extracting source names...");
            foreach (var source in sourcesList)
            {
                
                string sourceName = source.unique_site_name.ToLower();
                int minIndex = sourceName.Length;
                foreach (var subString in delimList.Select(r => r.ToLower()).AsEnumerable())
                {
                    var index = sourceName.IndexOf(subString);
                    if (index > -1 && index < minIndex)
                        minIndex = index;
                }
                string river = source.unique_site_name.Substring(0,minIndex).TrimEnd(',', ' ');
                river = ReplaceAcronyms(river);
                river = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(river.ToLower());
                source.feature_class = GetFeatureClass(river);
                source sourceWithRiver = source;
                sourceWithRiver.river = river;
                LogWriter.WriteMessageToLog($"{source.agency}-{source.agency_id}, source name: {source.river}, source type: {source.feature_class}");
                resultList.Add(sourceWithRiver);
            }
            return new SourceCollection(resultList);
        }

        /// <summary>
        /// Determines what type of 
        /// water source is being input;
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetFeatureClass(string riverName)
        {
            List<string> FeatureTypes = new List<string>() {"river","lake", "brook", "stream", "tributary", "pond", "creek", "canal", "branch","fork", "strand", "lock", "drain", "run", "brook", "reservoir", "sound", "channel", "strait", "estuary","bay", "lagoon"};
            string featureType = "";
            if (FeatureTypes.Any(f => riverName.ToLower().Contains(f.ToLower())))
                featureType = FeatureTypes.Where(f => riverName.ToLower().Contains(f.ToLower())).FirstOrDefault();

            return featureType;
        }

        /// <summary>
        /// Replace water source acronyms 
        /// ("R" -> "River", "Ck" -> "Creek", ext...)
        /// </summary>
        /// <param name="river"></param>
        /// <returns></returns>
        private string ReplaceAcronyms(string river)
        {
            Dictionary<string, string> acronymDictionary = new Dictionary<string, string>()
                {
                    {"ck", "Creek"},
                    {"cr", "Creek"},
                    { "riv", "River"},
                    { "rv", "River"},
                    { "rvr", "River"},
                    { "r", "River"},
                    { "fk", "Fork"}
                };
            foreach (var acronym in acronymDictionary)
            {
                string riverAcronym = new string(river.ToLower().Reverse().Take(acronym.Key.Length + 1).ToArray());
                char[] testArray = acronym.Key.ToLower().PadLeft(acronym.Key.Length + 1).ToCharArray();
                Array.Reverse(testArray);
                string testAcronym = new string(testArray);
                if (river.Length > acronym.Key.Length + 1 && riverAcronym == testAcronym)
                {
                    river = river.Remove(river.Length - acronym.Key.Length) + acronym.Value;
                    break;
                }
            }
            return river;
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
