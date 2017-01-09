using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RTI.DataBase.Updater;

namespace RTI.DataBase.UpdaterService
{
    public partial class RTIDBUpdaterService : ServiceBase
    {
        private static long itteration = 0;
        private CancellationToken quitToken;
        private CancellationTokenSource quitTokenSource = new CancellationTokenSource();
        private Task monitorTask;
        private static bool isRunning;
        private static string[] startArgs;

        public RTIDBUpdaterService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Attach the debugger for testing
            if (Application.Settings.DebugMode && !Debugger.IsAttached)
                Debugger.Launch();

            Logger.WriteToLog("RTI Database Updater initiated on " + DateTime.Now.ToShortDateString() + " @" + DateTime.Now.ToShortTimeString() + "\r\n\r\n");
            monitorTask = Task.Factory.StartNew(() => RunUpdater(quitToken), quitToken);
        }

        /// <summary>
        /// Main entry point of the
        /// application for both 
        /// Console and Service modes.
        /// </summary>
        /// <param name="quitToken"></param>
        private void RunUpdater(CancellationToken quitToken)
        {
            // Run DB Updater on Scheduled Interval
            UpdateManager manager = new UpdateManager();
            Scheduler scheduler = new Scheduler();
            //scheduler.RunOnSchedule(manager.RunUpdate);
            scheduler.RunOnSchedule(manager.RunUpdate);
        }

        public void OnStartConsole(string[] args)
        {

            Logger.WriteToLog("RTI Database Updater initiated on " + DateTime.Now.ToShortDateString() + " @" + DateTime.Now.ToShortTimeString() + "\r\n\r\n");
            RunUpdater(quitToken);
        }

        protected override void OnStop()
        {
            Logger.WriteToLog("Service Cancellation Requested");
            quitTokenSource.Cancel();
            monitorTask.Wait(60000);
            isRunning = false;
            quitTokenSource.Dispose();
            Logger.WriteToLog("Service Stopped");
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            if (powerStatus.HasFlag(PowerBroadcastStatus.QuerySuspend))
            {
                OnStop();
            }

            if (powerStatus.HasFlag(PowerBroadcastStatus.ResumeSuspend))
            {
                if (!isRunning)
                {
                    quitTokenSource = new CancellationTokenSource();
                    quitToken = quitTokenSource.Token;
                    OnStart(startArgs);
                }
            }
            return base.OnPowerEvent(powerStatus);
        }
    }
}
