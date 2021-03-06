using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public interface IDatabase : IDisposable
    {
        ICollection<Category> Categories { get; }
        ICollection<Habit> Habits { get; }
        ICollection<Time> Times { get; }
        ICollection<Settings> Settings { get; }

        Task SaveChanges();
    }

    public interface IDatabaseAccess
    {
        Task<IDatabase> CreateDatabase();
    }

    public class DataService : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        private string habitFilter = string.Empty;
        public string HabitFilter
        {
            get => habitFilter;
            set
            {
                if (habitFilter != value)
                {
                    habitFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public Settings Settings = new Settings();

        public List<Category> CategoryList { get; set; } = new List<Category>();
        public List<Habit> HabitList { get; set; } = new List<Habit>();
        public List<Time> TimeList { get; set; } = new List<Time>();

        public Dictionary<long, Category> CategoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Habit> HabitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Time> TimeDict { get; set; } = new Dictionary<long, Time>();

        readonly JsInterop JsInterop;
        readonly IDatabaseAccess DatabaseAccess;

        public DataService(JsInterop jsInterop, IDatabaseAccess databaseAccess)
        {
            JsInterop = jsInterop;
            DatabaseAccess = databaseAccess;
        }

        private IEnumerable<Habit> GetSorted(IEnumerable<Habit> habits)
        {
            return Settings.Sort switch
            {
                Sort.Index => habits.OrderBy(habit => habit.Id),
                Sort.Description => habits.OrderBy(habit => habit.Description),
                Sort.ElapsedTime => habits.OrderByDescending(habit => habit.ElapsedTime),
                Sort.SelectedRatio => habits.OrderByDescending(habit => habit.GetRatio(Settings.Ratio)),
                _ => throw new ArgumentException("Invalid argument: " + nameof(Settings.Sort))
            };
        }

        public IEnumerable<Habit> GetHabits(bool pinned, long categoryId)
        {
            IEnumerable<Habit> habits = HabitList.Where(habit =>
            {
                bool isRatioOk = habit.GetRatio(Settings.Ratio) >= Settings.ShowPercentMin;

                bool isDescriptionOk = string.IsNullOrEmpty(HabitFilter) || habit.Description.Contains(HabitFilter, StringComparison.OrdinalIgnoreCase);

                return isDescriptionOk && (habit.IsPinned == pinned) && (pinned || categoryId == 0 || habit.CategoryId == categoryId) && 
                        (
                            (
                                (habit.IsPinned || Settings.ShowPinned != true) && 
                                (habit.IsStarred || Settings.ShowStarred != true) &&
                                (habit.IsTwoMinute || Settings.ShowTwoMinute != true) && 
                                (habit.IsNeverDone || Settings.ShowNeverDone != true) && 
                                (habit.IsDoneOnce || Settings.ShowDoneOnce != true) &&
                                (isRatioOk || Settings.ShowRatioOverPercentMin != true)
                            )
                            || (habit.IsPinned && Settings.ShowPinned == null) 
                            || (habit.IsStarred && Settings.ShowStarred == null)
                            || (habit.IsTwoMinute && Settings.ShowTwoMinute == null) 
                            || (habit.IsNeverDone && Settings.ShowNeverDone == null) 
                            || (habit.IsDoneOnce && Settings.ShowDoneOnce == null) 
                            || (isRatioOk && Settings.ShowRatioOverPercentMin == null)
                        );
            });

            return GetSorted(habits);
        }

        public async Task SaveSettings()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            Settings settings = db.Settings.Single();

            settings.ShowPercentMin = Settings.ShowPercentMin;
            settings.ShowPinned = Settings.ShowPinned;
            settings.ShowStarred = Settings.ShowStarred;
            settings.ShowTwoMinute = Settings.ShowTwoMinute;
            settings.ShowNeverDone = Settings.ShowNeverDone;
            settings.ShowDoneOnce = Settings.ShowDoneOnce;
            settings.ShowRatioOverPercentMin = Settings.ShowRatioOverPercentMin;
            settings.ShowFilters = Settings.ShowFilters;
            settings.ShowAdvancedFilters = Settings.ShowAdvancedFilters;
            settings.ShowHabitId = Settings.ShowHabitId;
            settings.ShowHabitIdUpDownButtons = Settings.ShowHabitIdUpDownButtons;
            settings.ShowAllSelectOptions = Settings.ShowAllSelectOptions;
            settings.ShowCategories = Settings.ShowCategories;
            settings.ShowCategoriesInHeader = Settings.ShowCategoriesInHeader;
            settings.ShowSearch = Settings.ShowSearch;
            settings.ShowSort = Settings.ShowSort;
            settings.ShowPinStar2min = Settings.ShowPinStar2min;
            settings.ShowAverageInterval = Settings.ShowAverageInterval;
            settings.ShowDesiredInterval = Settings.ShowDesiredInterval;
            settings.ShowRatio = Settings.ShowRatio;
            settings.ShowRatioOptions = Settings.ShowRatioOptions;
            settings.ShowTimes = Settings.ShowTimes;
            settings.Size = Settings.Size;
            settings.Theme = Settings.Theme;
            settings.Ratio = Settings.Ratio;
            settings.Sort = Settings.Sort;

            await db.SaveChanges();

            OnPropertyChanged(nameof(Settings));
        }

        public async Task LoadData()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            bool save = false;

            if (db.Settings.Count == 0)
            {
                Dimensions dimensions = await JsInterop.GetDimensions();

                if (dimensions.Width < 576)
                    db.Settings.Add(new Settings() { Size = "small", Theme = "lumen" });
                else
                    db.Settings.Add(new Settings() { Size = "medium", Theme = "superhero" });

                save = true;
            }

            Settings = db.Settings.Single();

            if (db.Categories.Count == 0)
            {
                db.Categories.Add(new Category() { Id = 1, Description = "No category" });
                save = true;
            }

            if (save)
            {
                await db.SaveChanges();
            }

            CategoryList = db.Categories.ToList();
            HabitList = db.Habits.ToList();
            TimeList = db.Times.ToList();

            CategoryDict = CategoryList.ToDictionary(category => category.Id);
            HabitDict = HabitList.ToDictionary(habit => habit.Id);
            TimeDict = TimeList.ToDictionary(time => time.Id);

            foreach (Time time in TimeList)
            {
                if (HabitDict.ContainsKey(time.HabitId))
                    HabitDict[time.HabitId].TimeList.Add(time);
            }

            foreach (Habit habit in HabitList)
            {
                if (habit.TimeList.Count > 1)
                    habit.AverageInterval = TimeSpan.FromMilliseconds(habit.TimeList.Zip(habit.TimeList.Skip(1), (x, y) => (y.DateTime - x.DateTime).TotalMilliseconds).Average());

                //if (habit.DesiredInterval == TimeSpan.Zero)
                //    habit.DesiredInterval = new TimeSpan(1, 0, 0, 0);

                if (CategoryDict.ContainsKey(habit.CategoryId))
                    CategoryDict[habit.CategoryId].HabitList.Add(habit);
            }
        }

        //private async Task<Dimensions> GetDimensions()
        //{
        //    return await jsRuntime.InvokeAsync<Dimensions>("getDimensions");
        //}

        public async Task ClearData()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            foreach (Category category in db.Categories)
            {
                if (category.Id != 1)
                    db.Categories.Remove(category);
            }

            db.Habits.Clear();
            db.Times.Clear();

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }

        public async Task AddData(List<Category> categoryList)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            foreach (Category category in categoryList)
            {
                db.Categories.Add(category);

                foreach (Habit habit in category.HabitList)
                {
                    db.Habits.Add(habit);

                    foreach (Time time in habit.TimeList)
                    {
                        db.Times.Add(time);
                    }
                }
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }

        public async Task SaveCategory(Category category)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (category.Id == 0)
            {
                db.Categories.Add(category);
            }
            else if (db.Categories.SingleOrDefault(c => c.Id == category.Id) is Category dbCategory)
            {
                dbCategory.Description = category.Description;
                dbCategory.Color = category.Color;
                dbCategory.Icon = category.Icon;
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }

        public async Task DeleteCategory(Category category)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            foreach (Habit habit in category.HabitList)
            {
                habit.CategoryId = 1;

                if (db.Habits.SingleOrDefault(h => h.Id == habit.Id) is Habit dbHabit)
                    dbHabit.CategoryId = habit.CategoryId;
            }

            db.Categories.Remove(category);

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }

        public async Task SaveHabit(Habit habit)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (habit.Id == 0)
            {
                db.Habits.Add(habit);
            }
            else if (db.Habits.SingleOrDefault(h => h.Id == habit.Id) is Habit dbHabit)
            {
                dbHabit.CategoryId = habit.CategoryId;
                dbHabit.Description = habit.Description;
                dbHabit.IsPinned = habit.IsPinned;
                dbHabit.IsStarred = habit.IsStarred;
                dbHabit.IsTwoMinute = habit.IsTwoMinute;
                dbHabit.AverageIntervalTicks = habit.AverageIntervalTicks;
                dbHabit.DesiredIntervalTicks = habit.DesiredIntervalTicks;
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(HabitList));
        }

        public async Task DeleteHabit(Habit habit)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            foreach (Time time in habit.TimeList)
            {
                db.Times.Remove(time);
            }

            db.Habits.Remove(habit);

            await db.SaveChanges();

            await LoadData();
        }

        public async Task<(bool changed, long id)> HabitUpDown(long oldId, long newId)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

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

                await LoadData();
            }

            return (changed, newId);
        }

        private bool ChangeId(long oldId, long newId, IDatabase db)
        {
            if (db.Habits.SingleOrDefault(h => h.Id == oldId) is Habit dbHabit)
            {
                Habit habit = HabitDict[oldId];

                if (db.Habits.SingleOrDefault(h => h.Id == newId) is Habit otherHabit)
                {
                    otherHabit.Id = oldId;

                    foreach (Time time in HabitDict[newId].TimeList)
                    {
                        if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
                            dbTime.HabitId = oldId;
                    }

                    HabitDict[oldId] = HabitDict[newId];
                    HabitDict[newId] = habit;
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

        public async Task SaveTime(Time time)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (time.Id == 0)
            {
                db.Times.Add(time);
            }
            else if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
            {
                dbTime.DateTime = time.DateTime;
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(TimeList));
        }

        public async Task DeleteTime(Time time)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();
            db.Times.Remove(time);
            await db.SaveChanges();

            await LoadData();
        }

        public async Task SeedExamples()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            db.Categories.Add(new Category() { Id = 2, Description = "Health" });
            db.Categories.Add(new Category() { Id = 3, Description = "Exercise" });
            db.Categories.Add(new Category() { Id = 4, Description = "Appearance" });

            db.Categories.Add(new Category() { Id = 5, Description = "Peace of mind" });
            db.Categories.Add(new Category() { Id = 6, Description = "Relationships" });
            db.Categories.Add(new Category() { Id = 7, Description = "Relaxation" });

            db.Categories.Add(new Category() { Id = 8, Description = "Hobbies" });
            db.Categories.Add(new Category() { Id = 9, Description = "Chores" });
            db.Categories.Add(new Category() { Id = 10, Description = "Job" });

            db.Habits.Add(new Habit() { Id = 1, CategoryId = 2, Description = "Drink a glass of water" });
            db.Habits.Add(new Habit() { Id = 2, CategoryId = 2, Description = "Eat a piece of fruit" });

            db.Habits.Add(new Habit() { Id = 3, CategoryId = 3, Description = "Stretch & workout" });
            db.Habits.Add(new Habit() { Id = 4, CategoryId = 3, Description = "Go hiking" });

            db.Habits.Add(new Habit() { Id = 5, CategoryId = 4, Description = "Go to a hairdresser" });
            db.Habits.Add(new Habit() { Id = 6, CategoryId = 4, Description = "Buy new clothes" });

            db.Habits.Add(new Habit() { Id = 7, CategoryId = 5, Description = "Take a walk" });
            db.Habits.Add(new Habit() { Id = 8, CategoryId = 5, Description = "Meditate" });

            db.Habits.Add(new Habit() { Id = 9, CategoryId = 6, Description = "Call parents" });
            db.Habits.Add(new Habit() { Id = 10, CategoryId = 6, Description = "Do someone a favor" });

            db.Habits.Add(new Habit() { Id = 11, CategoryId = 7, Description = "Read a book" });
            db.Habits.Add(new Habit() { Id = 12, CategoryId = 7, Description = "Get a massage" });

            db.Habits.Add(new Habit() { Id = 13, CategoryId = 8, Description = "Learn Spanish" });
            db.Habits.Add(new Habit() { Id = 14, CategoryId = 8, Description = "Play the piano" });

            db.Habits.Add(new Habit() { Id = 15, CategoryId = 9, Description = "Clean dust under the bed" });
            db.Habits.Add(new Habit() { Id = 16, CategoryId = 9, Description = "Clean the windows" });

            db.Habits.Add(new Habit() { Id = 17, CategoryId = 10, Description = "Ask for a raise" });
            db.Habits.Add(new Habit() { Id = 18, CategoryId = 10, Description = "Take a break" });

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }
    }
}
