using System;
using System.Linq;

namespace Sledge.Common.Scheduling
{
    public class SerialisedSchedule
    {
        public ScheduleType Type { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int TimeInterval { get; set; }
        public int TimeToRun { get; set; }
        public string WeekDaysToRun { get; set; }
        public string MonthDaysToRun { get; set; }

        public static SerialisedSchedule Serialise(Schedule schedule)
        {
            return new SerialisedSchedule
            {
                Type = schedule.Type,
                ScheduleTime = schedule.ScheduleTime,
                TimeInterval = (int)schedule.TimeInterval.TotalSeconds,
                TimeToRun = (int)schedule.TimeToRun.TotalSeconds,
                WeekDaysToRun = String.Join(",", schedule.WeekDaysToRun),
                MonthDaysToRun = String.Join(",", schedule.MonthDaysToRun).Replace("-1", "Last")
            };
        }

        public static Schedule Deserialise(SerialisedSchedule schedule)
        {
            return new Schedule(schedule.Type)
            {
                ScheduleTime = schedule.ScheduleTime,
                TimeInterval = TimeSpan.FromSeconds(schedule.TimeInterval),
                TimeToRun = TimeSpan.FromSeconds(schedule.TimeToRun),
                WeekDaysToRun =
                    (schedule.WeekDaysToRun ?? "").Split(',')
                    .Where(x => !String.IsNullOrEmpty(x))
                    .Select(x => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), x))
                    .ToList(),
                MonthDaysToRun =
                    (schedule.MonthDaysToRun ?? "").Replace("Last", "-1")
                    .Split(',')
                    .Where(x => !String.IsNullOrEmpty(x))
                    .Select(int.Parse)
                    .ToList()
            };
        }
    }
}