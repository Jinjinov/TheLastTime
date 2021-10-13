using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Shared.Models
{
    public class Goal : IEntity<Goal>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        public List<Habit> HabitList = new List<Habit>();

        public List<Tasky> TaskList = new List<Tasky>();

        internal int Lines => Notes.Count(c => c == '\n') + 1; // Notes.Split(Environment.NewLine).Length;

        public void CopyTo(Goal goal)
        {
            goal.Description = Description;
            goal.Notes = Notes;
            goal.CategoryId = CategoryId;
        }
    }
}
