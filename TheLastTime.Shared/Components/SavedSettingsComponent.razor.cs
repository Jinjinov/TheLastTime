using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components
{
    public sealed partial class SavedSettingsComponent : IDisposable
    {
        public bool editSettings;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        protected override void OnInitialized()
        {
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
