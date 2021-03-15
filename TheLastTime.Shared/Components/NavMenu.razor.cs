using Microsoft.AspNetCore.Components;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components
{
    public partial class NavMenu
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        bool collapseNavMenu = true;

        string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

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
    }
}
