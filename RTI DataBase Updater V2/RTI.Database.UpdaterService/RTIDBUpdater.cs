using RTI.DataBase.Util;
using RTI.Database.UpdaterService;

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
