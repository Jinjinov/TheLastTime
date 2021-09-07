using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
{
    public class Category : IEntity<Category>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long GroupId { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;

        [Required]
        public string Icon { get; set; } = string.Empty;

        public List<Habit> HabitList = new List<Habit>();

        public List<Note> NoteList = new List<Note>();

        public List<ToDo> ToDoList = new List<ToDo>();

        public void CopyTo(Category category)
        {
            category.GroupId = GroupId;
            category.Description = Description;
            category.Color = Color;
            category.Icon = Icon;
        }
    }
}
