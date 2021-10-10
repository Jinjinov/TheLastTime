using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public sealed partial class NavMenu : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        bool _collapseNavMenu = true;

        string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

        Category _selectedNode = null!;
        IList<Category> _expandedNodes = new List<Category>();

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

        async void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;

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
            _collapseNavMenu = true;

            if (DataService.Settings.ShowHelp != true)
            {
                DataService.Settings.ShowHelp = true;
                await DataService.SaveSettings();
            }
        }

        async void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            _collapseNavMenu = true;

            if (DataService.Settings.ShowHelp != false)
            {
                DataService.Settings.ShowHelp = false;
                await DataService.SaveSettings();
            }
        }
    }
}
