using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace RTI.DataBase.Util
{
    /// <summary>
    /// Schedule a Method to 
    /// be run at a certain time.
    /// </summary>
    public class Scheduler
    {
        public DateTime SchedulerInitializationDate { get { return _intializationDate; } private set { } }
        public DateTime LastRunDate { get { return _lastRunDate; } private set { } }
        public DateTime NextRunDate { get { return _nextRunDate; } private set { } }
        private DateTime _intializationDate = new DateTime();
        private DateTime _lastRunDate = new DateTime();
        private DateTime _nextRunDate = new DateTime();
        private bool _jobComplete = false;
        private bool _firstRun = true;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        TimeSpan _runInterval;
        Action Job;

        public Scheduler()
        {
            _intializationDate = DateTime.UtcNow;
            _lastRunDate = _intializationDate;
        }


        /// <summary>
        /// Run an Action on a specified 
        /// interval. Use <paramref name="runSpan"/> to 
        /// override app.config schedule settings. 
        /// Set <paramref name="async"/> = <c>true</c> to run 
        /// the Action asynchronously. 
        /// </summary>
        /// <param name="job">Action to be executed.</param>
        /// <param name="async">Set to true to run code asynchronously</param>
        /// /// <param name="runSpan">Schedule override period.</param>
        public void RunOnSchedule(Action job, bool async = false, TimeSpan ? runSpan = null)
        {
            if (job != null)
            {
                Job = job;
                TimeSpan dueTime = GetWaitSpan(runSpan, out _runInterval);
                Timer timer;
                _nextRunDate = DateTime.Now.Add(dueTime);
                Logger.WriteToLog($"Next Scheduled Run at {_nextRunDate.ToString("MM/dd/yyyy hh:mm:ss tt")} ({_nextRunDate.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss UTC")})", Priority.Info);

                if (!Schedule.Settings.RunOnce && _runInterval != TimeSpan.MaxValue)
                {
                    timer = new Timer(RunJob, true, dueTime, _runInterval);
                    if (!async)
                        Thread.Sleep(Timeout.Infinite);
                }
                else
                {
                    timer = new Timer(RunJob, async, Convert.ToInt32(dueTime.TotalMilliseconds), Timeout.Infinite);
                    _waitHandle.WaitOne();
                }
            }
        }

        private void RunJob(object await)
        {
            if ((bool)await)
            {
                if (_jobComplete || _firstRun)
                {
                    _jobComplete = false;
                    _lastRunDate = DateTime.Now;
                    Task.Factory.StartNew(new Action(() => Job.Invoke()))
                        .ContinueWith(x => ResetTime(true));
                }
            }
            else
            {
                _jobComplete = false;
                Task task = new Task(() => Job.Invoke());
                _lastRunDate = DateTime.Now;
                task.Start();
                task.Wait();
                ResetTime(false);
                _waitHandle.Set();
            }
        }

        private void ResetTime(bool notifyNext)
        {
            _jobComplete = true;
            _firstRun = false;
            Logger.WriteToLog($"Job Completed at {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")} ({DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss UTC")})", Priority.Info);
            if (notifyNext)
            {
                _nextRunDate = _lastRunDate.Add(_runInterval);
                Logger.WriteToLog($"Next Scheduled Run at {_nextRunDate.ToString("MM/dd/yyyy hh:mm:ss tt")} ({_nextRunDate.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss UTC")})", Priority.Info);
            }

        }


        /// <summary>
        /// Gets the UTC Date of the 
        /// next scheduled run.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        private TimeSpan GetWaitSpan(TimeSpan? span, out TimeSpan period)
        {
            double IntervalMin = double.TryParse(Schedule.Settings.IntervalMinutes, out IntervalMin) ? IntervalMin : 15.00;
            int IntervalSec = Convert.ToInt32(Math.Round(60 * IntervalMin));
            TimeSpan dueTime = TimeSpan.Zero;
            if (span == null)
            {
                switch (Schedule.Settings.Mode)
                {
                    case "Scheduled":
                        period = new TimeSpan(0, 0, IntervalSec);
                        DateTime scheduledTime = DateTime.ParseExact(Schedule.Settings.ScheduledTime, "HH:mm:ss", CultureInfo.InvariantCulture);
                        if (scheduledTime > DateTime.Now)
                            dueTime = scheduledTime - DateTime.Now;
                        else
                            dueTime = (scheduledTime + TimeSpan.FromDays(1)) - DateTime.Now;
                        break;
                    case "Interval":
                        TimeSpan RunInterval = new TimeSpan(0, 0, IntervalSec);
                        dueTime = RunInterval;
                        period = dueTime;
                        break;
                    case "Manual":
                        dueTime = TimeSpan.FromMilliseconds(0);
                        period = TimeSpan.MaxValue;
                        break;
                    default:
                        dueTime = (DateTime.Today + TimeSpan.FromDays(1)) - DateTime.Now;
                        period = TimeSpan.FromDays(1);
                        break;
                }
            }
            else
            {
                dueTime = (TimeSpan)span;
                period = dueTime;
            }
            return dueTime;
        }
    }
}