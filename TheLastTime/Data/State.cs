using Blazorise;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheLastTime.Data
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

        readonly DataService DataService;

        public State(DataService dataService)
        {
            DataService = dataService;
        }

        readonly Dictionary<string, ButtonSize> ButtonSizeDict = new Dictionary<string, ButtonSize>()
        {
            { "small", ButtonSize.Small },
            { "medium", ButtonSize.None },
            { "large", ButtonSize.Large }
        };

        public ButtonSize ButtonSize => ButtonSizeDict[DataService.Settings.Size];

        readonly Dictionary<string, string> ButtonSizeClassDict = new Dictionary<string, string>()
        {
            { "small", "btn-sm" },
            { "medium", "" },
            { "large", "btn-lg" }
        };

        public string ButtonSizeClass => ButtonSizeClassDict[DataService.Settings.Size];

        readonly Dictionary<string, Size> SizeDict = new Dictionary<string, Size>()
        {
            { "small", Size.Small },
            { "medium", Size.None },
            { "large", Size.Large }
        };

        public Size Size => SizeDict[DataService.Settings.Size];

        public Category selectedCategory = new Category();
        private long _selectedCategoryId;
        public long selectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;

                    if (DataService.CategoryDict.ContainsKey(value))
                    {
                        selectedCategory = DataService.CategoryDict[value];

                        OnPropertyChanged(nameof(selectedCategory));
                    }
                }
            }
        }

        public Habit? selectedHabit;

        public Time? selectedTime;

        public void NextCategory()
        {
            int index = DataService.CategoryList.IndexOf(selectedCategory);

            if (index >= 0 && index < DataService.CategoryList.Count - 1)
            {
                selectedCategoryId = DataService.CategoryList[index + 1].Id;
            }
        }

        public void PreviousCategory()
        {
            int index = DataService.CategoryList.IndexOf(selectedCategory);

            if (index > 0 && index <= DataService.CategoryList.Count - 1)
            {
                selectedCategoryId = DataService.CategoryList[index - 1].Id;
            }
        }
    }
}
