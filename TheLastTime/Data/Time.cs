using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
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
