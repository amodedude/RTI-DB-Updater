﻿using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Application : ConfigurationSection
    {
        private static Application _settings = ConfigurationManager.GetSection("Application") as Application;

        public static Application Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("DownloadRepository", IsRequired = true)]
        public string DownloadRepository
        {
            get { return (string)this["DownloadRepository"]; }
        }

        [ConfigurationProperty("MaxDegreeOfParallelism", IsRequired = true)]
        public int MaxDegreeOfParallelism
        {
            get { return (int)this["MaxDegreeOfParallelism"]; }
        }
    }
}