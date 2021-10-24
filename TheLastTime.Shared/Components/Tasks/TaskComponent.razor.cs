using Microsoft.AspNetCore.Components;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components.Tasks
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

        private void OnDescriptionChanged(string value)
        {
            Task.Description = value;
        }

        private void OnNotesChanged(string value)
        {
            Task.Notes = value;
        }
    }
}
