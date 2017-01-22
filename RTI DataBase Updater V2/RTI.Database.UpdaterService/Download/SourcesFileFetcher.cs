﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.Database.UpdaterService.Parse;
using RTI.DataBase.Interfaces.Download;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.Download
{
    public class SourcesFileFetcher : IFileFetcher
    {
        public SourcesFileFetcher(ILogger logger, IDownloader downloader, string dir = null)
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
        private readonly IDownloader Downloader;

        public string CurrentFolder
        {
            get { return _currentFolder; }
            private set { }
        }

        private readonly string _currentFolder = string.Empty;
        private readonly string _sourcesFolder = string.Empty;
        private string _sourcesFile = string.Empty;

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

            // Return all sources that don't already exist in the database.
            var newSourcesList = new SourceCollection(downladedSources.Where(s => !sourceList.Contains(s)));
            return newSourcesList;
        }

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
