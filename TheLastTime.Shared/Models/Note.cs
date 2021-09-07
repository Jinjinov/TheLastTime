using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
{
    public class Note : IEntity<Note>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        public void CopyTo(Note note)
        {
            note.Title = Title;
            note.Text = Text;
            note.CategoryId = CategoryId;
        }
    }
}
