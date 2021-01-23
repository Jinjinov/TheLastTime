using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Data
{
    public class Habit
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public bool IsPinned { get; set; }

        [Required]
        public bool IsStarred { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public long AverageIntervalTicks { get => AverageInterval.Ticks; set => AverageInterval = new TimeSpan(value); }

        [Required]
        public long DesiredIntervalTicks { get => DesiredInterval.Ticks; set => DesiredInterval = new TimeSpan(value); }

        internal TimeSpan AverageInterval { get; set; }

        internal TimeSpan DesiredInterval { get; set; } = new TimeSpan(1, 0, 0, 0);

        public List<Time> TimeList = new List<Time>();

        internal TimeSpan SinceLastTime => TimeList.Any() ? DateTime.Now - TimeList.Last().DateTime : TimeSpan.Zero;

        internal bool IsAverageOverdue => (TimeList.Count > 1) && (SinceLastTime > AverageInterval);

        internal double AverageOverduePercent => TimeList.Count > 1 ? SinceLastTime / AverageInterval * 100.0 : 0.0;

        internal bool IsDesiredOverdue => (TimeList.Count > 1) && (SinceLastTime > DesiredInterval);

        internal double DesiredOverduePercent => TimeList.Count > 1 ? SinceLastTime / DesiredInterval * 100.0 : 0.0;

        internal TimeSpan GetInterval(Interval interval) => interval switch
        {
            Interval.Average => AverageInterval,
            Interval.Desired => DesiredInterval,
            _ => throw new ArgumentException("Invalid argument: " + nameof(interval))
        };

        internal bool IsOverdue(Interval interval) => interval switch
        {
            Interval.Average => IsAverageOverdue,
            Interval.Desired => IsDesiredOverdue,
            _ => throw new ArgumentException("Invalid argument: " + nameof(interval))
        };

        internal double OverduePercent(Interval interval) => interval switch
        {
            Interval.Average => AverageOverduePercent,
            Interval.Desired => DesiredOverduePercent,
            _ => throw new ArgumentException("Invalid argument: " + nameof(interval))
        };
    }
}
