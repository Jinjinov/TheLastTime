using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
{
    public class ToDo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }
    }
}
