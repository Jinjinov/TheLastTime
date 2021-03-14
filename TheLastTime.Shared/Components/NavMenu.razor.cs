using Microsoft.AspNetCore.Components;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components
{
    public partial class NavMenu
    {
        [Inject]
        State State { get; set; } = null!;

        bool collapseNavMenu = true;

        string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;

            State.ShowHelp = false;
            State.ShowOptions = false;
        }

        void ShowHelp()
        {
            State.ShowHelp = true;

            State.ShowOptions = false;
            collapseNavMenu = true;
        }

        void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            State.ShowHelp = false;
            collapseNavMenu = true;
        }
    }
}
