using RTI.DataBase.UpdaterService;
using RTI.DataBase.Util;

namespace RTI.DataBase.Updater
{
    class RTIDBUpdater 
    {
        static void Main(string[] args)
        {
            // Run the RTI Updater service as a console application. 
            Logger logger = new Logger();
            Emailer emailer = new Emailer(logger);
            RTIDBUpdaterService updater = new RTIDBUpdaterService(logger, emailer);
            updater.OnStartConsole(args);
        }
    }
}
