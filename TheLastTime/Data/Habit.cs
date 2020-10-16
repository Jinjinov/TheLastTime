using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    public class Habit
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long CategoryId { get; set; }

        public List<Time> TimeList = new List<Time>();
    }
}
