using Blazorise;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public bool Advanced { get; set; }

        readonly DataService DataService;

        public State(DataService dataService)
        {
            DataService = dataService;
        }

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

        public Category SelectedCategory = new Category();
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

                        OnPropertyChanged(nameof(SelectedCategory));
                    }
                }
            }
        }

        public Habit? SelectedHabit;

        public Time? SelectedTime;

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
