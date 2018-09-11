using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.Common.Scheduling
{
    /// <summary>
    /// Represents a schedule that may run once or on an interval.
    /// </summary>
    public class Schedule
    {
        public Schedule(ScheduleType type)
        {
            Type = type;
            LastRun = null;
            RunCount = 0;
        }

        public ScheduleType Type { get; set; }
        public DateTime? LastRun { get; set; }
        public int RunCount { get; set; }

        // Once/Interval
        public DateTime ScheduleTime { get; set; }

        // Interval
        public TimeSpan TimeInterval { get; set; }

        // Daily/Weekly/Monthly
        public TimeSpan TimeToRun { get; set; }

        // Weekly
        public List<DayOfWeek> WeekDaysToRun { get; set; }

        // Monthly
        public List<int> MonthDaysToRun { get; set; }

        public static Schedule Once(DateTime time)
        {
            return new Schedule(ScheduleType.Once) { ScheduleTime = time };
        }

        /// <summary>
        /// Create an interval schedule with a given start time.
        /// </summary>
        /// <param name="time">The time to run the task the first time</param>
        /// <param name="interval">The interval between executions. Note that this must be over 10 seconds.</param>
        /// <returns>The resulting schedule</returns>
        public static Schedule Interval(DateTime time, TimeSpan interval)
        {
            return new Schedule(ScheduleType.Interval) { ScheduleTime = time, TimeInterval = interval };
        }

        /// <summary>
        /// Create an interval schedule.
        /// </summary>
        /// <param name="interval">The interval between executions. Note that this must be over 10 seconds.</param>
        /// <returns>The resulting schedule</returns>
        public static Schedule Interval(TimeSpan interval)
        {
            return new Schedule(ScheduleType.Interval) { ScheduleTime = DateTime.Now, TimeInterval = interval };
        }

        /// <summary>
        /// Create a schedule that runs once a day.
        /// </summary>
        /// <param name="timeToRun">The time of the day to run this schedule.</param>
        /// <returns>The resulting schedule</returns>
        public static Schedule Daily(TimeSpan timeToRun)
        {
            return new Schedule(ScheduleType.Daily) { TimeToRun = timeToRun };
        }

        /// <summary>
        /// Create a schedule that runs once a week.
        /// </summary>
        /// <param name="timeToRun">The time of the day to run this schedule.</param>
        /// <param name="daysToRun">The days of the week to run this schedule</param>
        /// <returns>The resulting schedule</returns>
        public static Schedule Weekly(TimeSpan timeToRun, IEnumerable<DayOfWeek> daysToRun)
        {
            return new Schedule(ScheduleType.Weekly) { TimeToRun = timeToRun, WeekDaysToRun = daysToRun.ToList() };
        }

        /// <summary>
        /// Create a schedule that runs once a month.
        /// </summary>
        /// <param name="timeToRun">The time of the day to run this schedule.</param>
        /// <param name="daysToRun">The days of the month to run this schedule</param>
        /// <returns>The resulting schedule</returns>
        public static Schedule Monthly(TimeSpan timeToRun, IEnumerable<int> daysToRun)
        {
            return new Schedule(ScheduleType.Monthly) { TimeToRun = timeToRun, MonthDaysToRun = daysToRun.ToList() };
        }

        /// <summary>
        /// Get the next time that this schedule is due to run.
        /// </summary>
        /// <returns>Next run time or null if the schedule won't run again</returns>
        public DateTime? NextScheduleTime()
        {
            var now = DateTime.Now;
            var today = now.Date.Add(TimeToRun);
            switch (Type)
            {
                case ScheduleType.Once:
                    if (RunCount > 0 || ScheduleTime <= now) return null;
                    return ScheduleTime;
                case ScheduleType.Interval:
                    if (now < ScheduleTime) return ScheduleTime;
                    if (LastRun != null) return LastRun.Value.Add(TimeInterval);
                    var t = ScheduleTime;
                    while (t <= now) t = t.Add(TimeInterval.TotalSeconds < 10 ? TimeSpan.FromSeconds(10) : TimeInterval);
                    return t;
                case ScheduleType.Daily:
                    return now > today ? today.AddDays(1) : today;
                case ScheduleType.Weekly:
                    var nwd = NextWeeklyDate(now > today ? now.Date.AddDays(1) : now.Date);
                    return nwd?.Add(TimeToRun);
                case ScheduleType.Monthly:
                    var nmd = NextMonthlyDate(now > today ? now.Date.AddDays(1) : now.Date);
                    return nmd?.Add(TimeToRun);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DateTime? NextWeeklyDate(DateTime fromDate)
        {
            for (var i = 0; i < 7; i++)
            {
                var dt = fromDate.Date.AddDays(i);
                if (WeekDaysToRun.Contains(dt.DayOfWeek)) return dt;
            }
            return null;
        }

        private DateTime? NextMonthlyDate(DateTime fromDate)
        {
            var last = MonthDaysToRun.Contains(-1);
            for (var i = 0; i < 32; i++)
            {
                var dt = fromDate.Date.AddDays(i);
                if (last && dt.Day == 1 && i > 0) return dt.AddDays(-1);
                if (MonthDaysToRun.Contains(dt.Day)) return dt;
            }
            return null;
        }
    }
}