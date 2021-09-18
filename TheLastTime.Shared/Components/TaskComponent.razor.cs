using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public partial class TaskComponent
    {
        [Parameter]
        public ToDo ToDo { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        async Task OnDescriptionChanged(string value)
        {
            ToDo.Description = value;

            await DataService.Save(ToDo);
        }

        async Task OnNotesChanged(string value)
        {
            ToDo.Notes = value;

            await DataService.Save(ToDo);
        }
    }
}
