using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    public class Category
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Habit> HabitList = new List<Habit>();
    }
}
