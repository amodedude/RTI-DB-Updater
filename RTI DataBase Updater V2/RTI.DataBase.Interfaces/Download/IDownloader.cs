namespace RTI.DataBase.Interfaces.Download
{
    public interface IFileDownloader
    {
        void download_file(string uri, string filePath, bool useCompression);
    }
}
