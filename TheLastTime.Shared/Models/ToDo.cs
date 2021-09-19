using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Shared.Models
{
    public class ToDo : IEntity<ToDo>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        internal int NotesLines => Notes.Count(c => c == '\n') + 1; // Notes.Split(Environment.NewLine).Length;

        public void CopyTo(ToDo toDo)
        {
            toDo.Description = Description;
            toDo.Notes = Notes;
            toDo.CategoryId = CategoryId;
        }
    }
}
