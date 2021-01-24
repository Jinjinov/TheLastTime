using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Data
{
    public enum Interval
    {
        Average,
        Desired
    }

    public enum Sort
    {
        Index,
        Description,
        ElapsedTime,
        ElapsedPercent,
        AverageToDesiredRatio
    }

    public class Settings
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ShowPercentMin { get; set; }

        [Required]
        public bool ShowOnlyStarred { get; set; }

        [Required]
        public bool ShowOnlyTwoMinute { get; set; }

        [Required]
        public bool ShowOnlyOverdue { get; set; }

        [Required]
        public bool ShowHabitId { get; set; }

        [Required]
        public bool ShowHabitIdUpDownButtons { get; set; }

        [Required]
        public bool ShowAllSelectOptions { get; set; }

        [Required]
        public bool ShowCategoriesInHeader { get; set; }

        [Required]
        public string Size { get; set; } = string.Empty;

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public Interval Interval { get; set; }

        [Required]
        public Sort Sort { get; set; }
    }
}
