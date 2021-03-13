using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
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
        public bool? ShowPinned { get; set; } = false;

        [Required]
        public bool? ShowStarred { get; set; } = false;

        [Required]
        public bool? ShowTwoMinute { get; set; } = false;

        [Required]
        public bool? ShowNeverDone { get; set; } = false;

        [Required]
        public bool? ShowDoneOnce { get; set; } = false;

        [Required]
        public bool? ShowRatioOverPercentMin { get; set; } = false;

        [Required]
        public bool ShowFilters { get; set; }

        [Required]
        public bool ShowAdvancedFilters { get; set; }

        [Required]
        public bool ShowHabitId { get; set; }

        [Required]
        public bool ShowHabitIdUpDownButtons { get; set; }

        [Required]
        public bool ShowAllSelectOptions { get; set; }

        [Required]
        public bool ShowCategories { get; set; }

        [Required]
        public bool ShowCategoriesInHeader { get; set; }

        [Required]
        public bool ShowSearch { get; set; } = true;

        [Required]
        public bool ShowSort { get; set; }

        [Required]
        public bool ShowPinStar2min { get; set; }

        [Required]
        public bool ShowAverageInterval { get; set; }

        [Required]
        public bool ShowDesiredInterval { get; set; }

        [Required]
        public bool ShowRatio { get; set; }

        [Required]
        public bool ShowRatioOptions { get; set; }

        [Required]
        public bool ShowTimes { get; set; }

        [Required]
        public string Size { get; set; } = "medium";

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public Ratio Ratio { get; set; }

        [Required]
        public Sort Sort { get; set; }

        public void SetShowAdvancedFilters(bool showAdvancedFilters)
        {
            ShowAdvancedFilters = showAdvancedFilters;

            if (!ShowAdvancedFilters)
            {
                if (ShowPinned == null)
                    ShowPinned = false;

                if (ShowStarred == null)
                    ShowStarred = false;

                if (ShowTwoMinute == null)
                    ShowTwoMinute = false;

                if (ShowNeverDone == null)
                    ShowNeverDone = false;

                if (ShowDoneOnce == null)
                    ShowDoneOnce = false;

                if (ShowRatioOverPercentMin == null)
                    ShowRatioOverPercentMin = false;
            }
        }
    }
}
