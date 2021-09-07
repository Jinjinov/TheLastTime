using System.ComponentModel.DataAnnotations;

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

        public void CopyTo(ToDo toDo)
        {
            toDo.Description = Description;
            toDo.Notes = Notes;
            toDo.CategoryId = CategoryId;
        }
    }
}
