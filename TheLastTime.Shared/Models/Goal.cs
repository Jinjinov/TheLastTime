using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheLastTime.Shared.Models
{
    public class Goal : IEntity<Goal>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        internal int Lines => Text.Count(c => c == '\n') + 1; // Notes.Split(Environment.NewLine).Length;

        public void CopyTo(Goal goal)
        {
            goal.Title = Title;
            goal.Text = Text;
            goal.CategoryId = CategoryId;
        }
    }
}
