using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sledge.Common.Scheduling
{
    /// <summary>
    /// A simple timer-based scheduled task runner.
    /// </summary>
    public static class Scheduler
    {
        private static readonly List<ScheduledCallback> Callbacks;
        private static readonly Timer Timer;

        private static readonly object Lock = new object();

        static Scheduler()
        {
            Callbacks = new List<ScheduledCallback>();
            Timer = new Timer();
            Timer.Elapsed += (s, e) => TimerCallback();
        }

        /// <summary>
        /// Schedule a task to run after a number of milliseconds
        /// </summary>
        /// <typeparam name="T">Context type</typeparam>
        /// <param name="context">Context</param>
        /// <param name="callback">The task to run</param>
        /// <param name="milliseconds">The number of milliseconds to run after</param>
        public static void Schedule<T>(T context, Action callback, long milliseconds)
        {
            Schedule(context, callback, DateTime.Now.AddMilliseconds(milliseconds));
        }

        public static void Schedule<T>(T context, Action callback, TimeSpan span)
        {
            Schedule(context, callback, DateTime.Now.Add(span));
        }

        public static void Schedule<T>(T context, Action callback, DateTime date)
        {
            Schedule(context, callback, Sledge.Common.Scheduling.Schedule.Once(date));
        }

        public static void Schedule<T>(T context, Action callback, Schedule schedule)
        {
            lock (Lock)
            {
                Callbacks.Add(new ScheduledCallback(context, schedule, callback));
                UpdateTimer();
            }
        }

        public static T GetContext<T>(Func<T, bool> filter)
        {
            lock (Lock)
            {
                return Callbacks.Select(x => x.Context).OfType<T>().FirstOrDefault(filter);
            }
        }

        public static void RemoveContext<T>(Func<T, bool> filter)
        {
            lock (Lock)
            {
                Callbacks.RemoveAll(x => x.Context is T variable && filter(variable));
                UpdateTimer();
            }
        }

        public static void OverrideScheduleTime<T>(Func<T, bool> contextFilter, long milliseconds)
        {
            OverrideScheduleTime(contextFilter, DateTime.Now.AddMilliseconds(milliseconds));
        }

        public static void OverrideScheduleTime<T>(Func<T, bool> contextFilter, TimeSpan time)
        {
            OverrideScheduleTime(contextFilter, DateTime.Now.Add(time));
        }

        public static void OverrideScheduleTime<T>(Func<T, bool> contextFilter, DateTime newTime)
        {
            lock (Lock)
            {
                foreach (var sch in Callbacks.Where(x => x.Context is T variable && contextFilter(variable)))
                {
                    sch.SetScheduleTime(newTime);
                }
                UpdateTimer();
            }
        }

        public static void Clear()
        {
            lock (Lock)
            {
                Callbacks.Clear();
                UpdateTimer();
            }
        }

        private static void UpdateTimer()
        {
            if (!Callbacks.Any(x => x.ScheduledTime.HasValue))
            {
                Timer.Stop();
            }
            else
            {
                var next = Callbacks.Where(x => x.ScheduledTime.HasValue).OrderBy(x => x.ScheduledTime).First();
                var delay = Math.Max(0, DateTime.Now.Subtract(next.ScheduledTime.Value).TotalMilliseconds) + 100;
                Timer.Interval = delay;
                Timer.Start();
            }
        }

        private static void TimerCallback()
        {
            lock (Lock)
            {
                var expired = Callbacks.Where(x => x.ScheduledTime.HasValue && x.ScheduledTime <= DateTime.Now).ToList();
                expired.ForEach(x => x.Run());
                Callbacks.RemoveAll(x => !x.ScheduledTime.HasValue);
                UpdateTimer();
            }
        }

        private class ScheduledCallback
        {
            public ScheduledCallback(object context, Schedule schedule, Action callback)
            {
                Context = context;
                Schedule = schedule;
                Callback = callback;

                ScheduledTime = Schedule.NextScheduleTime();
            }

            public object Context { get; private set; }
            public Schedule Schedule { get; private set; }
            public DateTime? ScheduledTime { get; private set; }
            public Action Callback { get; private set; }

            public void SetScheduleTime(DateTime time)
            {
                ScheduledTime = time;
            }

            public void Run()
            {
                Callback();
                ScheduledTime = Schedule.NextScheduleTime();
            }
        }
    }
}
