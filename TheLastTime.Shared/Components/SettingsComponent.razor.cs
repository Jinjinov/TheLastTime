using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public sealed partial class SettingsComponent : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        Sort Sort
        {
            get => DataService.Settings.Sort;
            set
            {
                if (DataService.Settings.Sort != value)
                {
                    DataService.Settings.Sort = value;
                    DataService.SaveSettings().Wait();
                }
            }
        }

        async Task PreviousSort()
        {
            if (DataService.Settings.Sort > Sort.Index)
            {
                DataService.Settings.Sort -= 1;
                await DataService.SaveSettings();
            }
            else if (DataService.Settings.Sort == Sort.Index)
            {
                DataService.Settings.Sort = Sort.SelectedRatio;
                await DataService.SaveSettings();
            }
        }

        async Task NextSort()
        {
            if (DataService.Settings.Sort < Sort.SelectedRatio)
            {
                DataService.Settings.Sort += 1;
                await DataService.SaveSettings();
            }
            else if (DataService.Settings.Sort == Sort.SelectedRatio)
            {
                DataService.Settings.Sort = Sort.Index;
                await DataService.SaveSettings();
            }
        }

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

        string GetFilter(bool? state)
        {
            if (DataService.Settings.ShowAdvancedFilters)
            {
                return state switch
                {
                    false => "Show",
                    true => "Show only",
                    null => "Also show",
                };
            }
            else
            {
                return "Show only";
            }
        }
    }
}
