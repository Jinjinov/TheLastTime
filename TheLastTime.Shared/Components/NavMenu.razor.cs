using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components
{
    public sealed partial class NavMenu : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        bool collapseNavMenu = true;

        string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

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
            collapseNavMenu = !collapseNavMenu;

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
            collapseNavMenu = true;

            if (DataService.Settings.ShowHelp != true)
            {
                DataService.Settings.ShowHelp = true;
                await DataService.SaveSettings();
            }
        }

        async void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            collapseNavMenu = true;

            if (DataService.Settings.ShowHelp != false)
            {
                DataService.Settings.ShowHelp = false;
                await DataService.SaveSettings();
            }
        }

        public class Item
        {
            public string Text { get; set; } = string.Empty;
            public IEnumerable<Item>? Children { get; set; }
        }

        readonly IEnumerable<Item> Items = new[]
        {
            new Item { Text = "Item 1" },
            new Item
            {
                Text = "Item 2",
                Children = new []
                {
                    new Item { Text = "Item 2.1" },
                    new Item
                    { 
                        Text = "Item 2.2", 
                        Children = new []
                        {
                            new Item { Text = "Item 2.2.1" },
                            new Item { Text = "Item 2.2.2" },
                            new Item { Text = "Item 2.2.3" },
                            new Item { Text = "Item 2.2.4" }
                        }
                    },
                    new Item { Text = "Item 2.3" },
                    new Item { Text = "Item 2.4" }
                }
            },
            new Item { Text = "Item 3" },
        };

        IList<Item> ExpandedNodes = new List<Item>();
        Item selectedNode;
    }
}
