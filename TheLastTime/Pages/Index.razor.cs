using Blazorise;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public sealed partial class Index : IDisposable
    {
        public Category selectedCategory = new Category();
        public long selectedCategoryId;

        public Habit? selectedHabit;

        public Time? selectedTime;

        public bool editCategory;
        public bool editHabit;
        public bool editTime;

        [Inject]
        DataService DataService { get; set; } = null!;

        readonly Dictionary<string, ButtonSize> ButtonSizeDict = new Dictionary<string, ButtonSize>()
        {
            { "small", ButtonSize.Small },
            { "medium", ButtonSize.None },
            { "large", ButtonSize.Large }
        };

        ButtonSize ButtonSize => ButtonSizeDict[DataService.Settings.Size];

        readonly Dictionary<string, string> ButtonSizeClassDict = new Dictionary<string, string>()
        {
            { "small", "btn-sm" },
            { "medium", "" },
            { "large", "btn-lg" }
        };

        string ButtonSizeClass => ButtonSizeClassDict[DataService.Settings.Size];

        readonly Dictionary<string, Size> SizeDict = new Dictionary<string, Size>()
        {
            { "small", Size.Small },
            { "medium", Size.None },
            { "large", Size.Large }
        };

        Size Size => SizeDict[DataService.Settings.Size];

        [Parameter]
        public string? SeedExamples { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (SeedExamples == "examples")
            {
                await DataService.SeedExamples();
            }

            DataService.PropertyChanged += DataService_PropertyChanged;
        }

        void DataService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= DataService_PropertyChanged;
        }
    }
}
