using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public class State : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        public bool EditHabit { get; set; }

        private bool showOptions;
        public bool ShowOptions
        {
            get => showOptions;
            set
            {
                if (showOptions != value)
                {
                    showOptions = value;
                    OnPropertyChanged();
                }
            }
        }

        public Category SelectedCategory { get; private set; } = new Category();

        private long _selectedCategoryId;
        public long SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;

                    if (DataService.CategoryDict.ContainsKey(value))
                    {
                        SelectedCategory = DataService.CategoryDict[value];
                    }

                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        private Habit? _selectedHabit;
        public Habit? SelectedHabit
        {
            get => _selectedHabit;
            set
            {
                if (_selectedHabit != value)
                {
                    _selectedHabit = value;
                    OnPropertyChanged();
                }
            }
        }

        public Time? SelectedTime;

        readonly DataService DataService;

        public State(DataService dataService)
        {
            DataService = dataService;
        }

        public void NewHabit()
        {
            EditHabit = true;
            SelectedHabit = new Habit { CategoryId = SelectedCategoryId != 0 ? SelectedCategoryId : 1 };
        }

        public void SetSelectedHabit(long habitId)
        {
            if (SelectedHabit != null)
            {
                if (DataService.HabitDict.ContainsKey(habitId))
                    SelectedHabit = DataService.HabitDict[habitId];
                else
                    SelectedHabit = DataService.HabitList.LastOrDefault();
            }
        }

        private async Task SaveHabit(Habit habit)
        {
            await DataService.SaveHabit(habit);

            SetSelectedHabit(habit.Id);
        }

        public async Task SetDesiredIntervalDays(int days)
        {
            if (SelectedHabit == null)
                return;

            SelectedHabit.DesiredInterval = new TimeSpan(days, SelectedHabit.DesiredInterval.Hours, SelectedHabit.DesiredInterval.Minutes, SelectedHabit.DesiredInterval.Seconds);

            await SaveHabit(SelectedHabit);
        }

        public async Task SetDesiredIntervalHours(int hours)
        {
            if (SelectedHabit == null)
                return;

            SelectedHabit.DesiredInterval = new TimeSpan(SelectedHabit.DesiredInterval.Days, hours, SelectedHabit.DesiredInterval.Minutes, SelectedHabit.DesiredInterval.Seconds);

            await SaveHabit(SelectedHabit);
        }

        public void NewCategory()
        {
            SelectedCategory = new Category();

            if (DataService.CategoryList.Any())
                _selectedCategoryId = DataService.CategoryList.Last().Id + 1;
        }

        public void NextCategory()
        {
            int index = DataService.CategoryList.FindIndex(category => category.Id == SelectedCategoryId);

            if (index >= 0 && index < DataService.CategoryList.Count - 1)
            {
                SelectedCategoryId = DataService.CategoryList[index + 1].Id;
            }
            else if (DataService.CategoryList.Any())
            {
                SelectedCategoryId = DataService.CategoryList.First().Id;
            }
        }

        public void PreviousCategory()
        {
            int index = DataService.CategoryList.FindIndex(category => category.Id == SelectedCategoryId);

            if (index > 0 && index <= DataService.CategoryList.Count - 1)
            {
                SelectedCategoryId = DataService.CategoryList[index - 1].Id;
            }
            else if (DataService.CategoryList.Any())
            {
                SelectedCategoryId = DataService.CategoryList.Last().Id;
            }
        }
    }
}
