using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public sealed partial class Index : IDisposable
    {
        public bool editCategory;
        public bool editHabit;
        public bool editTime;

        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Parameter]
        public string? SeedExamples { get; set; }

        protected async override Task OnInitializedAsync()
        {
            string query = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;

            if (SeedExamples == "examples" || query == "?examples")
            {
                await DataService.SeedExamples();
            }

            DataService.PropertyChanged += PropertyChanged;
            State.PropertyChanged += PropertyChanged;
        }

        void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= PropertyChanged;
            State.PropertyChanged -= PropertyChanged;
        }

        //public static string ToReadableString(TimeSpan span)
        //{
        //    return span.TotalMinutes >= 1.0 ? (
        //        (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
        //        (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
        //        (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
        //        ) : "0 minutes";
        //}

        public static string ToReadableString(TimeSpan span)
        {
            return span.Days > 0 ? span.Days + " day" + (span.Days == 1 ? string.Empty : "s") 
                                 : span.Hours > 0 ? span.Hours + " hour" + (span.Hours == 1 ? string.Empty : "s") 
                                                  : span.Minutes + " minute" + (span.Minutes == 1 ? string.Empty : "s");
        }
    }
}
