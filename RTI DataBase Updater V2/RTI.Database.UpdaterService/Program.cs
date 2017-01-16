using RTI.DataBase.Util;
using System.ServiceProcess;

namespace RTI.DataBase.UpdaterService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            
            Logger logger = new Logger();
            Emailer emailer = new Emailer(logger);
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RTIDBUpdaterService(logger, emailer)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
