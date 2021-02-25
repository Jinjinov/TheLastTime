using Microsoft.AspNetCore.Components;
using TheLastTime.Data;

namespace TheLastTime.Shared
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

            State.ShowOptions = false;
        }

        void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            collapseNavMenu = true;
        }
    }
}
