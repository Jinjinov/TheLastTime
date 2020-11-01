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
        NavigationManager NavigationManager { get; set; } = null!;

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
            string query = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;

            if (SeedExamples == "examples" || query == "?examples")
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

        public static string ToReadableString(TimeSpan span)
        {
            return span.TotalMinutes >= 1.0 ? (
                (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
                ) : "0 m";
        }
    }
}
