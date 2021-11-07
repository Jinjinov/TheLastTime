using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components.Settings
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

        async Task DeleteSettings()
        {
            // TODO: sync DataService list with db list
            await DataService.DeleteSettings(DataService.Settings);

            DataService.SettingsId = 1;
        }
    }
}
