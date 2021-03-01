using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public sealed partial class SelectedHabitComponent : IDisposable
    {
        public bool editTime;

        public long newHabitId = -1;

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

        private async Task SaveHabit(Habit habit)
        {
            await DataService.SaveHabit(habit);

            State.SetSelectedHabit(habit.Id);
        }

        private async Task HabitUpDown(long oldId, long newId)
        {
            var (changed, id) = await DataService.HabitUpDown(oldId, newId);

            if (changed)
                State.SetSelectedHabit(id);
        }

        public static string ToReadableString(TimeSpan span)
        {
            return span.TotalMinutes >= 1.0 ? (
                (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
                ) : "0 minutes";
        }
    }
}
