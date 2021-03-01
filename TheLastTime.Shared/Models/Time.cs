using System;
using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
{
    public class Time
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public long HabitId { get; set; }
    }
}
