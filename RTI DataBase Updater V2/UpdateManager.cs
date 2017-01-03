using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
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

            
        }
    }
}