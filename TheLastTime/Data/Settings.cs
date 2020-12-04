using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Data
{
    public class Settings
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ShowPercentMin { get; set; }

        [Required]
        public bool ShowOnlyStarred { get; set; }

        [Required]
        public bool ShowOnlyOverdue { get; set; }

        [Required]
        public string Size { get; set; } = string.Empty;

        [Required]
        public string Theme { get; set; } = string.Empty;
    }
}
