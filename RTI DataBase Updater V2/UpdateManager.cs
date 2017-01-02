using System;
using System.Threading;

namespace RTI.DataBase.Updater
{
    internal class UpdateManager
    {
        internal void RunUpdate()
        {
            Console.WriteLine("Performing DB update...");
            Thread.Sleep(5000);
            Console.WriteLine("DB Update Finished!\r\n");
        }
    }
}