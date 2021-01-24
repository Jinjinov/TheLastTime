using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Data
{
    public enum Ratio
    {
        ElapsedToAverage,
        ElapsedToDesired,
        AverageToDesired
    }

    public enum Sort
    {
        Index,
        Description,
        ElapsedTime,
        SelectedRatio
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
        public bool ShowOnlyRatioOverOne { get; set; }

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
        public Ratio Ratio { get; set; }

        [Required]
        public Sort Sort { get; set; }
    }
}
