using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Threading.Tasks;
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

        async Task SaveTask()
        {
            // TODO: sync DataService list with db list
            await DataService.Save(Task);

            State.EditTask = false;
        }
    }
}
