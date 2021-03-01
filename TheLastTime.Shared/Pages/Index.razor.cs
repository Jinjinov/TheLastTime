using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Pages
{
    public sealed partial class Index : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        //[Inject]
        //NavigationManager NavigationManager { get; set; } = null!;

        //[Parameter]
        //public string? RouteParameter { get; set; }

        protected override void OnInitialized()
        {
            //string query = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;

            //if (RouteParameter == "examples" || query == "?examples")
            //{
            //    await DataService.SeedExamples();
            //}

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
    }
}
