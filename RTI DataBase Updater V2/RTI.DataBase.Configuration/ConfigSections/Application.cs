using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Application : ConfigurationSection
    {
        public static Application Settings { get; } = ConfigurationManager.GetSection("Application") as Application;

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

        [ConfigurationProperty("DownloadTimeOutMilliseconds", IsRequired = true)]
        public int DownloadTimeOutMilliseconds
        {
            get { return (int)this["DownloadTimeOutMilliseconds"]; }
        }

        [ConfigurationProperty("UploadTimeOutMilliseconds", IsRequired = true)]
        public int UploadTimeOutMilliseconds
        {
            get { return (int)this["UploadTimeOutMilliseconds"]; }
        }

        [ConfigurationProperty("UseLatestCachedFiles", IsRequired = true)]
        public bool UseLatestCachedFiles
        {
            get { return (bool)this["UseLatestCachedFiles"]; }
        }

        [ConfigurationProperty("DebugMode", IsRequired = true)]
        public bool DebugMode
        {
            get { return (bool)this["DebugMode"]; }
        }
    }
}
