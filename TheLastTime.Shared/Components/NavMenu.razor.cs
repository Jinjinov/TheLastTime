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

            State.ShowOptions = false;
        }

        void ToggleOptions()
        {
            State.ShowOptions = !State.ShowOptions;

            collapseNavMenu = true;
        }
    }
}
