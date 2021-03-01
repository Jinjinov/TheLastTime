using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public sealed partial class HabitComponent : IDisposable
    {
        [Parameter]
        public bool Pinned { get; set; }

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

        async Task OnHabitDone(Habit habit)
        {
            if (habit.IsPinned)
            {
                habit.IsPinned = false;

                await DataService.SaveHabit(habit);
            }

            await DataService.SaveTime(new Time { HabitId = habit.Id, DateTime = DateTime.Now });

            State.SetSelectedHabit(habit.Id);
        }

        public static string ToHighestValueString(TimeSpan span)
        {
            return span.Days > 0 ? span.Days + " day" + (span.Days == 1 ? string.Empty : "s")
                                 : span.Hours > 0 ? span.Hours + " hour" + (span.Hours == 1 ? string.Empty : "s")
                                                  : span.Minutes + " minute" + (span.Minutes == 1 ? string.Empty : "s");
        }
    }
}
