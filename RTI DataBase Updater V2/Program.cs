using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Util;

namespace RTI.DataBase.Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            // Run DB Updater on Scheduled Interval
            UpdateManager manager = new UpdateManager();
            Scheduler scheduler = new Scheduler();
            //scheduler.RunOnSchedule(manager.RunUpdate);
            scheduler.RunOnSchedule(manager.RunUpdate);
        }
    }
}
