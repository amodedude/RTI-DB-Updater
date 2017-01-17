using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces;

namespace RTI.DataBase.UpdaterService
{
    public partial class RTIDBUpdaterService : ServiceBase
    {
        private CancellationToken quitToken;
        private CancellationTokenSource quitTokenSource = new CancellationTokenSource();
        private Task monitorTask;
        private static bool isRunning;
        public ILogger LogWriter;
        public IEmailer Emailer;

        public RTIDBUpdaterService(ILogger logger, IEmailer emailer)
        {
            Emailer = emailer;
            LogWriter = logger;
            InitializeComponent();
            quitToken = quitTokenSource.Token;
        }

        public void OnStartConsole(string[] args)
        {
            LogWriter.WriteMessageToLog("RTI Database Updater initiated on " + DateTime.Now.ToShortDateString() + " @" + DateTime.Now.ToShortTimeString() + "\r\n\r\n");
            RunUpdater(quitToken);
        }

        private void RunUpdater(CancellationToken quitToken)
        {
            Scheduler scheduler = new Scheduler(LogWriter);
            UpdateManager manager = new UpdateManager(LogWriter, Emailer);
            scheduler.RunOnSchedule(() => manager.RunUpdate());
            LogWriter.WriteMessageToLog("RTI Database Updater has completed on " + DateTime.Now.ToShortDateString() + " @" + DateTime.Now.ToShortTimeString());

        }

        protected override void OnStart(string[] args)
        {
            // Attach the debugger for testing
            if (Application.Settings.DebugMode && !Debugger.IsAttached)
                Debugger.Launch();

            LogWriter.WriteMessageToLog("RTI Database Updater initiated on " + DateTime.Now.ToShortDateString() + " @" + DateTime.Now.ToShortTimeString() + "\r\n\r\n");
            monitorTask = Task.Factory.StartNew(() => RunUpdater(quitToken), quitToken);
        }

        protected override void OnStop()
        {
            LogWriter.WriteMessageToLog("Service Cancellation Requested");
            quitTokenSource.Cancel();
            monitorTask.Wait(60000);
            isRunning = false;
            quitTokenSource.Dispose();
            LogWriter.WriteMessageToLog("Service Stopped");
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
                    OnStart(null);
                }
            }
            return base.OnPowerEvent(powerStatus);
        }
    }
}
