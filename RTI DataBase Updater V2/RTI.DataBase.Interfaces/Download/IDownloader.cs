namespace RTI.DataBase.Interfaces.Download
{
    public interface IDownloader
    {
        void download_file(string uri, string filePath, bool useCompression);
    }
}
