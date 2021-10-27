using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components.Goals
{
    public partial class SelectedGoalComponent
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

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

        void SelectedHabitValueChanged(long val)
        {
            State.SelectedGoal.HabitList.Add(DataService.HabitDict[val]);
            State.SelectedGoalSelectedHabitId = 0;
            State.ShowSelectedGoalHabits = false;
            StateHasChanged();
        }

        void SelectedTaskValueChanged(long val)
        {
            State.SelectedGoal.TaskList.Add(DataService.TaskDict[val]);
            State.SelectedGoalSelectedTaskId = 0;
            State.ShowSelectedGoalTasks = false;
            StateHasChanged();
        }
    }
}
