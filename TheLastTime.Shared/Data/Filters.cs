using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public class Filters : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        private string descriptionFilter = string.Empty;
        public string DescriptionFilter
        {
            get => descriptionFilter;
            set
            {
                if (descriptionFilter != value)
                {
                    descriptionFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? dateFilter = null;
        public DateTime? DateFilter
        {
            get => dateFilter;
            set
            {
                if (dateFilter != value)
                {
                    dateFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        readonly DataService DataService;

        public Filters(DataService dataService)
        {
            DataService = dataService;
        }

        private IEnumerable<Habit> GetSorted(IEnumerable<Habit> habits)
        {
            return DataService.Settings.Sort switch
            {
                Sort.Index => habits.OrderBy(habit => habit.Id),
                Sort.Description => habits.OrderBy(habit => habit.Description),
                Sort.ElapsedTime => habits.OrderByDescending(habit => habit.ElapsedTime),
                Sort.SelectedRatio => habits.OrderByDescending(habit => habit.GetRatio(DataService.Settings.Ratio)),
                _ => throw new ArgumentException("Invalid argument: " + nameof(DataService.Settings.Sort))
            };
        }

        public IEnumerable<Habit> GetHabits(bool pinned, long categoryId)
        {
            IEnumerable<Habit> habits = DataService.HabitList.Where(habit =>
            {
                bool isRatioOk = habit.GetRatio(DataService.Settings.Ratio) >= DataService.Settings.ShowPercentMin;

                bool isDescriptionOk = string.IsNullOrEmpty(DescriptionFilter) || habit.Description.Contains(DescriptionFilter, StringComparison.OrdinalIgnoreCase);

                bool isDateOk = DateFilter == null || habit.TimeList.Any(time => time.DateTime.Date == DateFilter?.Date);

                return isDescriptionOk && isDateOk && (habit.IsPinned == pinned) && (pinned || categoryId == 0 || habit.CategoryId == categoryId) &&
                        (
                            (
                                (habit.IsPinned || DataService.Settings.ShowPinned != true) &&
                                (habit.IsStarred || DataService.Settings.ShowStarred != true) &&
                                (habit.IsTwoMinute || DataService.Settings.ShowTwoMinute != true) &&
                                (habit.IsNeverDone || DataService.Settings.ShowNeverDone != true) &&
                                (habit.IsDoneOnce || DataService.Settings.ShowDoneOnce != true) &&
                                (isRatioOk || DataService.Settings.ShowRatioOverPercentMin != true)
                            )
                            || (habit.IsPinned && DataService.Settings.ShowPinned == null)
                            || (habit.IsStarred && DataService.Settings.ShowStarred == null)
                            || (habit.IsTwoMinute && DataService.Settings.ShowTwoMinute == null)
                            || (habit.IsNeverDone && DataService.Settings.ShowNeverDone == null)
                            || (habit.IsDoneOnce && DataService.Settings.ShowDoneOnce == null)
                            || (isRatioOk && DataService.Settings.ShowRatioOverPercentMin == null)
                        );
            });

            return GetSorted(habits);
        }
    }
}
