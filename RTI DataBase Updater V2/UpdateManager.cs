using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RTI.DataBase.Updater
{
    internal class UpdateManager
    {
        internal void RunUpdate()
        {
            Console.WriteLine("Performing DB update...");

            // Download all USGS Sources
            FileFetcher fetcher = new FileFetcher();
            HashSet<source> sources = fetcher.fetchFiles();

            // Upload to RTI DataBase
            UploadFiles(sources, fetcher);
        }

        /// <summary>
        /// Parse and Upload 
        /// data for each water 
        /// source.
        /// </summary>
        private void UploadFiles(HashSet<source> sources, FileFetcher fetcher)
        {
            FileParser parser = new FileParser();
            foreach(source source in sources)
            {
                string path = Path.Combine(fetcher.CurrentFolder, source.agency_id+".txt");
                if(File.Exists(path))
                    parser.ReadFile(path, source.agency_id);
            }
        }
    }
}