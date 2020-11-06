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
        public TimeSpan DesiredInterval { get; set; }

        public List<Time> TimeList = new List<Time>();

        internal TimeSpan SinceLastTime => DateTime.Now - TimeList.Last().DateTime;

        internal bool IsOverdue => (TimeList.Count > 1) && (SinceLastTime > DesiredInterval);
    }
}
