using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components.Habits
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

        private void OnNotesChanged(string value)
        {
            if (State.SelectedHabit != null)
            {
                State.SelectedHabit.Notes = value;

                State.SelectedHabit.NotesMarkdownHtml = DataService.MarkdownToHtml(State.SelectedHabit.Notes);
            }
        }

        public static string ToReadableString(TimeSpan span)
        {
            return span.TotalMinutes >= 1.0 ? (
                (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
                (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
                ) : "0 minutes";
        }

        private async Task SaveHabit(Habit habit)
        {
            await DataService.SaveHabit(habit);

            State.SetSelectedHabit(habit.Id);
        }

        private async Task HabitUpDown(long oldId, long newId)
        {
            var (changed, id) = await HabitUpDown2(oldId, newId);

            if (changed)
                State.SetSelectedHabit(id);
        }

        public async Task<(bool changed, long id)> HabitUpDown2(long oldId, long newId)
        {
            using IDatabase db = await DataService.DatabaseAccess.CreateDatabase();

            long maxId = db.Habits.Any() ? db.Habits.Max(habit => habit.Id) : 0;

            newId = Math.Clamp(newId, 1, maxId);

            bool changed = false;

            if (oldId < newId)
            {
                for (long i = oldId; i < newId; ++i)
                {
                    if (ChangeId(i, i + 1, db))
                    {
                        changed = true;
                    }
                }
            }

            if (oldId > newId)
            {
                for (long i = oldId; i > newId; --i)
                {
                    if (ChangeId(i, i - 1, db))
                    {
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                await db.SaveChanges();

                await DataService.LoadData();
            }

            return (changed, newId);
        }

        private bool ChangeId(long oldId, long newId, IDatabase db)
        {
            if (db.Habits.SingleOrDefault(h => h.Id == oldId) is Habit dbHabit)
            {
                Habit habit = DataService.HabitDict[oldId];

                if (db.Habits.SingleOrDefault(h => h.Id == newId) is Habit otherHabit)
                {
                    otherHabit.Id = oldId;

                    foreach (Time time in DataService.HabitDict[newId].TimeList)
                    {
                        if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
                            dbTime.HabitId = oldId;
                    }

                    DataService.HabitDict[oldId] = DataService.HabitDict[newId];
                    DataService.HabitDict[newId] = habit;
                }

                dbHabit.Id = newId;

                foreach (Time time in habit.TimeList)
                {
                    if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
                        dbTime.HabitId = newId;
                }

                return true;
            }

            return false;
        }
    }
}
