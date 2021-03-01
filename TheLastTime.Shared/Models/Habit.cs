using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Shared.Models
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
        public bool IsTwoMinute { get; set; }

        [Required]
        public long AverageIntervalTicks { get => AverageInterval.Ticks; set => AverageInterval = new TimeSpan(value); }

        [Required]
        public long DesiredIntervalTicks { get => DesiredInterval.Ticks; set => DesiredInterval = new TimeSpan(value); }

        internal TimeSpan AverageInterval { get; set; }

        internal TimeSpan DesiredInterval { get; set; } = new TimeSpan(1, 0, 0, 0);

        public List<Time> TimeList = new List<Time>();

        internal TimeSpan ElapsedTime => TimeList.Any() ? DateTime.Now - TimeList.Last().DateTime : TimeSpan.Zero;

        internal bool IsNeverDone => TimeList.Count == 0;

        internal bool IsDoneOnce => TimeList.Count == 1;

        internal bool IsElapsedOverAverage => (TimeList.Count > 1) && (ElapsedTime > AverageInterval);

        internal double ElapsedToAverageRatio => TimeList.Count > 1 ? ElapsedTime / AverageInterval * 100.0 : 0.0;

        internal bool IsElapsedOverDesired => (TimeList.Count > 1) && (ElapsedTime > DesiredInterval);

        internal double ElapsedToDesiredRatio => TimeList.Count > 1 ? ElapsedTime / DesiredInterval * 100.0 : 0.0;

        //internal bool IsRatioOverOne(Ratio ratio) => ratio switch
        //{
        //    Ratio.ElapsedToAverage => IsElapsedOverAverage,
        //    Ratio.ElapsedToDesired => IsElapsedOverDesired,
        //    Ratio.AverageToDesired => AverageInterval > DesiredInterval,
        //    _ => throw new ArgumentException("Invalid argument: " + nameof(ratio))
        //};

        internal double GetRatio(Ratio ratio) => ratio switch
        {
            Ratio.ElapsedToAverage => ElapsedToAverageRatio,
            Ratio.ElapsedToDesired => ElapsedToDesiredRatio,
            Ratio.AverageToDesired => AverageInterval / DesiredInterval * 100.0,
            _ => throw new ArgumentException("Invalid argument: " + nameof(ratio))
        };
    }
}
