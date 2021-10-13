using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Shared.Models
{
    public class Tasky : IEntity<Tasky>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        [Required]
        public long GoalId { get; set; }

        internal int NotesLines => Notes.Count(c => c == '\n') + 1; // Notes.Split(Environment.NewLine).Length;

        public void CopyTo(Tasky task)
        {
            task.Description = Description;
            task.Notes = Notes;
            task.CategoryId = CategoryId;
        }
    }
}
