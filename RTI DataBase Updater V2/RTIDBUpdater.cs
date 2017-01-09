using RTI.DataBase.UpdaterService;

namespace RTI.DataBase.Updater
{
    class RTIDBUpdater
    {
        static void Main(string[] args)
        {
            // Run the RTI Updater service as a console application. 
            RTIDBUpdaterService updater = new RTIDBUpdaterService();
            updater.OnStartConsole(args);
        }
    }
}
