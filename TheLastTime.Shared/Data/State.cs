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

        public bool EditNote { get; set; }

        public bool EditTask { get; set; }

        private bool _showOptions;
        public bool ShowOptions
        {
            get => _showOptions;
            set
            {
                if (_showOptions != value)
                {
                    _showOptions = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedTab = "Habits";
        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    OnPropertyChanged();
                }
            }
        }

        private Category _selectedCategory = new Category();
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;

                    DataService.Settings.SelectedCategoryId = _selectedCategory.Id;

                    if (DataService.CategoryDict.ContainsKey(_selectedCategory.Id))
                    {
                        SelectedCategoryIdx = DataService.CategoryList.IndexOf(SelectedCategory);
                    }

                    //OnPropertyChanged(nameof(SelectedCategory));
                    Task.Run(DataService.SaveSettings);
                }
            }
        }

        public long SelectedCategoryIdx { get; private set; }

        //private long _selectedCategoryId;
        public long SelectedCategoryId
        {
            get => DataService.Settings.SelectedCategoryId;
            set
            {
                if (DataService.Settings.SelectedCategoryId != value)
                {
                    DataService.Settings.SelectedCategoryId = value;

                    if (DataService.CategoryDict.ContainsKey(value))
                    {
                        _selectedCategory = DataService.CategoryDict[value];

                        SelectedCategoryIdx = DataService.CategoryList.IndexOf(SelectedCategory);
                    }

                    //OnPropertyChanged(nameof(SelectedCategory));
                    Task.Run(DataService.SaveSettings);
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

        public long SelectedGoalSelectedHabitId { get; set; }

        private Goal? _selectedGoal;
        public Goal? SelectedGoal
        {
            get => _selectedGoal;
            set
            {
                if (_selectedGoal != value)
                {
                    _selectedGoal = value;
                    OnPropertyChanged();
                }
            }
        }

        private Tasky? _selectedTask;
        public Tasky? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
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
            _selectedCategory = new Category() { CategoryId = 1 };

            if (DataService.CategoryList.Any())
                DataService.Settings.SelectedCategoryId = DataService.CategoryList.Last().Id + 1;
        }

        public void NextCategory()
        {
            int index = DataService.CategoryList.FindIndex(category => category.Id == SelectedCategoryId);

            if (index == DataService.CategoryList.Count - 1)
            {
                SelectedCategoryId = 0;
            }
            else if (index >= 0 && index < DataService.CategoryList.Count - 1)
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

            if (index == 0)
            {
                SelectedCategoryId = 0;
            }
            else if (index > 0 && index <= DataService.CategoryList.Count - 1)
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
