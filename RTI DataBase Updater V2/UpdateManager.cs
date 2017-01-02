using System;
using System.Threading;

namespace RTI.DataBase.Updater
{
    internal class UpdateManager
    {
        internal void RunUpdate()
        {
            Console.WriteLine("Performing DB update...");
            FileFetcher fetcher = new FileFetcher();
            fetcher.fetchFiles();
        }
    }
}