using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Sledge.Editor
{
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

        public static void Schedule(object context, Action callback, long milliseconds)
        {
            Schedule(context, callback, DateTime.Now.AddMilliseconds(milliseconds));
        }

        public static void Schedule(object context, Action callback, TimeSpan span)
        {
            Schedule(context, callback, DateTime.Now.Add(span));
        }

        public static void Schedule(object context, Action callback, DateTime date)
        {
            lock (Lock)
            {
                Callbacks.Add(new ScheduledCallback(context, date, callback));
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

        public static void Clear(object context)
        {
            lock (Lock)
            {
                Callbacks.RemoveAll(x => x.Context == context);
                UpdateTimer();
            }
        }

        private static void UpdateTimer()
        {
            if (!Callbacks.Any())
            {
                Timer.Stop();
            }
            else
            {
                var next = Callbacks.OrderBy(x => x.ScheduledTime).First();
                var delay = Math.Max(0, DateTime.Now.Subtract(next.ScheduledTime).TotalMilliseconds) + 100;
                Timer.Interval = delay;
                Timer.Start();
            }
        }

        private static void TimerCallback()
        {
            lock (Lock)
            {
                var expired = Callbacks.Where(x => x.ScheduledTime <= DateTime.Now).ToList();
                expired.ForEach(x => x.Callback());
                Callbacks.RemoveAll(expired.Contains);
                UpdateTimer();
            }
        }

        private class ScheduledCallback
        {
            public object Context { get; set; }
            public DateTime ScheduledTime { get; private set; }
            public Action Callback { get; private set; }

            public ScheduledCallback(object context, DateTime scheduledTime, Action callback)
            {
                Context = context;
                ScheduledTime = scheduledTime;
                Callback = callback;
            }
        }
    }
}
