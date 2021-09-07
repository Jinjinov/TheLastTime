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

    public class Settings : IEntity<Settings>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long SelectedSettingsId { get; set; }

        [Required]
        public bool ShowSavedSettings { get; set; }

        [Required]
        public long SelectedCategoryId { get; set; }

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
        public bool ShowHelp { get; set; } = true;

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
        public bool ShowDateFilter { get; set; }

        [Required]
        public bool ShowSort { get; set; }

        [Required]
        public bool ShowPinStar2min { get; set; }

        [Required]
        public bool ShowNotes { get; set; }

        [Required]
        public bool ShowAverageInterval { get; set; } = true;

        [Required]
        public bool ShowDesiredInterval { get; set; }

        [Required]
        public bool ShowRatio { get; set; } = true;

        [Required]
        public bool ShowRatioOptions { get; set; }

        [Required]
        public bool ShowTimes { get; set; }

        [Required]
        public bool BackupToGoogleDrive { get; set; }

        [Required]
        public string Size { get; set; } = "medium";

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public Ratio Ratio { get; set; }

        [Required]
        public Sort Sort { get; set; }

        public void CopyTo(Settings settings)
        {
            settings.Description = Description;
            settings.SelectedSettingsId = SelectedSettingsId;

            settings.ShowSavedSettings = ShowSavedSettings;
            settings.SelectedCategoryId = SelectedCategoryId;
            settings.ShowPercentMin = ShowPercentMin;

            settings.ShowPinned = ShowPinned;
            settings.ShowStarred = ShowStarred;
            settings.ShowTwoMinute = ShowTwoMinute;
            settings.ShowNeverDone = ShowNeverDone;
            settings.ShowDoneOnce = ShowDoneOnce;
            settings.ShowRatioOverPercentMin = ShowRatioOverPercentMin;

            settings.ShowHelp = ShowHelp;
            settings.ShowFilters = ShowFilters;
            settings.ShowAdvancedFilters = ShowAdvancedFilters;
            settings.ShowHabitId = ShowHabitId;
            settings.ShowHabitIdUpDownButtons = ShowHabitIdUpDownButtons;
            settings.ShowAllSelectOptions = ShowAllSelectOptions;
            settings.ShowCategories = ShowCategories;
            settings.ShowCategoriesInHeader = ShowCategoriesInHeader;
            settings.ShowSearch = ShowSearch;
            settings.ShowDateFilter = ShowDateFilter;
            settings.ShowSort = ShowSort;
            settings.ShowPinStar2min = ShowPinStar2min;
            settings.ShowNotes = ShowNotes;
            settings.ShowAverageInterval = ShowAverageInterval;
            settings.ShowDesiredInterval = ShowDesiredInterval;
            settings.ShowRatio = ShowRatio;
            settings.ShowRatioOptions = ShowRatioOptions;
            settings.ShowTimes = ShowTimes;
            settings.BackupToGoogleDrive = BackupToGoogleDrive;

            settings.Size = Size;
            settings.Theme = Theme;
            settings.Ratio = Ratio;
            settings.Sort = Sort;
        }

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
