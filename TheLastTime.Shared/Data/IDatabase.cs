using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public interface IDatabase : IDisposable
    {
        ICollection<Category> Categories { get; }
        ICollection<Habit> Habits { get; }
        ICollection<Note> Notes { get; }
        ICollection<Settings> Settings { get; }
        ICollection<Time> Times { get; }
        ICollection<ToDo> ToDos { get; }

        ICollection<T> GetCollection<T>();

        Task SaveChanges();

        async Task Seed(string size, string theme)
        {
            Categories.Add(new Category() { Id = 2, CategoryId = 1, Description = "Health" });
            Categories.Add(new Category() { Id = 3, CategoryId = 1, Description = "Exercise" });
            Categories.Add(new Category() { Id = 4, CategoryId = 1, Description = "Appearance" });

            Categories.Add(new Category() { Id = 5, CategoryId = 1, Description = "Peace of mind" });
            Categories.Add(new Category() { Id = 6, CategoryId = 1, Description = "Relationships" });
            Categories.Add(new Category() { Id = 7, CategoryId = 1, Description = "Relaxation" });

            Categories.Add(new Category() { Id = 8, CategoryId = 1, Description = "Hobbies" });
            Categories.Add(new Category() { Id = 9, CategoryId = 1, Description = "Chores" });
            Categories.Add(new Category() { Id = 10, CategoryId = 1, Description = "Job" });

            Habits.Add(new Habit() { Id = 1, CategoryId = 2, Description = "Drink a glass of water", DesiredInterval = new TimeSpan(0, 8, 0, 0), IsStarred = true, IsTwoMinute = true });
            Habits.Add(new Habit() { Id = 2, CategoryId = 2, Description = "Eat a piece of fruit", DesiredInterval = new TimeSpan(0, 12, 0, 0), IsTwoMinute = true });

            Habits.Add(new Habit() { Id = 3, CategoryId = 3, Description = "Stretch & workout", IsStarred = true });
            Habits.Add(new Habit() { Id = 4, CategoryId = 3, Description = "Go hiking", DesiredInterval = new TimeSpan(7, 0, 0, 0) });

            Habits.Add(new Habit() { Id = 5, CategoryId = 4, Description = "Go to a hairdresser", DesiredInterval = new TimeSpan(21, 0, 0, 0) });
            Habits.Add(new Habit() { Id = 6, CategoryId = 4, Description = "Buy new clothes", DesiredInterval = new TimeSpan(56, 0, 0, 0) });

            Habits.Add(new Habit() { Id = 7, CategoryId = 5, Description = "Take a walk" });
            Habits.Add(new Habit() { Id = 8, CategoryId = 5, Description = "Meditate", IsStarred = true });

            Habits.Add(new Habit() { Id = 9, CategoryId = 6, Description = "Call parents", IsStarred = true });
            Habits.Add(new Habit() { Id = 10, CategoryId = 6, Description = "Do someone a favor", DesiredInterval = new TimeSpan(14, 0, 0, 0) });

            Habits.Add(new Habit() { Id = 11, CategoryId = 7, Description = "Read a book", IsStarred = true });
            Habits.Add(new Habit() { Id = 12, CategoryId = 7, Description = "Get a massage", DesiredInterval = new TimeSpan(28, 0, 0, 0), IsStarred = true, IsPinned = true });

            Habits.Add(new Habit() { Id = 13, CategoryId = 8, Description = "Learn Spanish" });
            Habits.Add(new Habit() { Id = 14, CategoryId = 8, Description = "Play the piano" });

            Habits.Add(new Habit() { Id = 15, CategoryId = 9, Description = "Clean dust under the bed", DesiredInterval = new TimeSpan(14, 0, 0, 0) });
            Habits.Add(new Habit() { Id = 16, CategoryId = 9, Description = "Clean the windows", DesiredInterval = new TimeSpan(56, 0, 0, 0) });

            Habits.Add(new Habit() { Id = 17, CategoryId = 10, Description = "Ask for a raise", DesiredInterval = new TimeSpan(182, 0, 0, 0), IsPinned = true });
            Habits.Add(new Habit() { Id = 18, CategoryId = 10, Description = "Take a break", DesiredInterval = new TimeSpan(0, 8, 0, 0), IsTwoMinute = true });

            Times.Add(new Time() { Id = 1, HabitId = 5, DateTime = DateTime.Now.AddDays(-50) });
            Times.Add(new Time() { Id = 2, HabitId = 5, DateTime = DateTime.Now.AddDays(-28) });

            Times.Add(new Time() { Id = 3, HabitId = 12, DateTime = DateTime.Now.AddDays(-28) });
            Times.Add(new Time() { Id = 4, HabitId = 12, DateTime = DateTime.Now.AddDays(-7) });

            Times.Add(new Time() { Id = 5, HabitId = 15, DateTime = DateTime.Now.AddDays(-27) });
            Times.Add(new Time() { Id = 6, HabitId = 15, DateTime = DateTime.Now.AddDays(-13) });

            Times.Add(new Time() { Id = 7, HabitId = 16, DateTime = DateTime.Now.AddDays(-70) });
            Times.Add(new Time() { Id = 8, HabitId = 16, DateTime = DateTime.Now.AddDays(-12) });

            Times.Add(new Time() { Id = 9, HabitId = 17, DateTime = DateTime.Now.AddDays(-300) });

            Settings.Add(new Settings() { Description = "Hide", ShowSavedSettings = true, Size = size, Theme = theme, ShowHelp = false, ShowSearch = false, ShowAverageInterval = false, ShowRatio = false });
            Settings.Add(new Settings()
            {
                Description = "Show",
                ShowSavedSettings = true,
                Size = size,
                Theme = theme,
                ShowHelp = false,
                ShowFilters = true,
                ShowAdvancedFilters = true,
                ShowHabitId = true,
                ShowHabitIdUpDownButtons = true,
                ShowCategories = true,
                ShowCategoriesInHeader = true,
                ShowSearch = true,
                ShowDateFilter = true,
                ShowSort = true,
                ShowPinStar2min = true,
                ShowNotes = true,
                ShowAverageInterval = true,
                ShowDesiredInterval = true,
                ShowRatio = true,
                ShowRatioOptions = true,
                ShowTimes = true,
            });

            await SaveChanges();
        }
    }

    public interface IDatabaseAccess
    {
        Task<IDatabase> CreateDatabase();
    }
}
