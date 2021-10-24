using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public sealed partial class Sidebar : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        bool _collapseSidebar = true;

        string? SidebarCssClass => _collapseSidebar ? "collapse" : null;

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

        async void ToggleSidebar()
        {
            _collapseSidebar = !_collapseSidebar;

            State.ShowOptions = false;

            if (DataService.Settings.ShowHelp != false)
            {
                DataService.Settings.ShowHelp = false;
                await DataService.SaveSettings();
            }
        }

        async void ShowHelp()
        {
            State.ShowOptions = false;
            _collapseSidebar = true;

            if (DataService.Settings.ShowHelp != true)
            {
                DataService.Settings.ShowHelp = true;
                await DataService.SaveSettings();
            }
        }

        async void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            _collapseSidebar = true;

            if (DataService.Settings.ShowHelp != false)
            {
                DataService.Settings.ShowHelp = false;
                await DataService.SaveSettings();
            }
        }
    }
}
