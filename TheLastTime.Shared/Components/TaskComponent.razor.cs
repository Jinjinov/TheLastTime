using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public partial class TaskComponent
    {
        [Parameter]
        public Tasky Task { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        async Task OnDescriptionChanged(string value)
        {
            Task.Description = value;

            await DataService.Save(Task);
        }

        async Task OnNotesChanged(string value)
        {
            Task.Notes = value;

            await DataService.Save(Task);
        }
    }
}
