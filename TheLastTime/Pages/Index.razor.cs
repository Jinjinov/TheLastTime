using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public sealed partial class Index : IDisposable
    {
        public bool editCategory;
        public bool editHabit;
        public bool editTime;

        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Parameter]
        public string? RouteParameter { get; set; }

        protected override async Task OnInitializedAsync()
        {
            string query = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;

            if (RouteParameter == "examples" || query == "?examples")
            {
                await DataService.SeedExamples();
            }

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

        async Task OnHabitDone(Habit habit)
        {
            if (habit.IsPinned)
            {
                habit.IsPinned = false;

                await DataService.SaveHabit(habit);
            }

            await DataService.SaveTime(new Time { HabitId = habit.Id, DateTime = DateTime.Now });

            SetSelectedHabit(habit.Id);
        }

        private void SetSelectedHabit(long habitId)
        {
            if (State.SelectedHabit != null)
            {
                if (DataService.HabitDict.ContainsKey(habitId))
                    State.SelectedHabit = DataService.HabitDict[habitId];
                else
                    State.SelectedHabit = DataService.HabitList.LastOrDefault();
            }
        }

        private async Task SaveHabit(Habit habit)
        {
            await DataService.SaveHabit(habit);

            SetSelectedHabit(habit.Id);
        }

        private async Task HabitUp(Habit habit)
        {
            if (await DataService.HabitUp(habit))
                SetSelectedHabit(habit.Id - 1);
        }

        private async Task HabitDown(Habit habit)
        {
            if (await DataService.HabitDown(habit))
                SetSelectedHabit(habit.Id + 1);
        }

        public static string ToReadableString(TimeSpan span)
        {
            return span.TotalMinutes >= 1.0 ? (
                (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
                ) : "0 minutes";
        }

        public static string ToHighestValueString(TimeSpan span)
        {
            return span.Days > 0 ? span.Days + " day" + (span.Days == 1 ? string.Empty : "s") 
                                 : span.Hours > 0 ? span.Hours + " hour" + (span.Hours == 1 ? string.Empty : "s") 
                                                  : span.Minutes + " minute" + (span.Minutes == 1 ? string.Empty : "s");
        }
    }
}
