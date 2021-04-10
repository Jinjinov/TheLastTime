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

        private string descriptionFilter = string.Empty;
        public string DescriptionFilter
        {
            get => descriptionFilter;
            set
            {
                if (descriptionFilter != value)
                {
                    descriptionFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? dateFilter = null;
        public DateTime? DateFilter
        {
            get => dateFilter;
            set
            {
                if (dateFilter != value)
                {
                    dateFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public long SettingsId
        {
            get => Settings.SelectedSettingsId;
            set
            {
                if (Settings.SelectedSettingsId != value)
                {
                    if (SettingsDict.ContainsKey(value))
                    {
                        Settings = SettingsDict[value];
                    }

                    Settings.SelectedSettingsId = value;

                    //OnPropertyChanged(nameof(Settings));
                    Task.Run(SaveSettings);
                }
            }
        }
        public Settings Settings { get; private set; } = new Settings();

        public List<Category> CategoryList { get; set; } = new List<Category>();
        public List<Habit> HabitList { get; set; } = new List<Habit>();
        public List<Settings> SettingsList { get; set; } = new List<Settings>();
        public List<Time> TimeList { get; set; } = new List<Time>();

        public Dictionary<long, Category> CategoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Habit> HabitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Settings> SettingsDict { get; set; } = new Dictionary<long, Settings>();
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

                bool isDescriptionOk = string.IsNullOrEmpty(DescriptionFilter) || habit.Description.Contains(DescriptionFilter, StringComparison.OrdinalIgnoreCase);

                bool isDateOk = DateFilter == null || habit.TimeList.Any(time => time.DateTime.Date == DateFilter?.Date);

                return isDescriptionOk && isDateOk && (habit.IsPinned == pinned) && (pinned || categoryId == 0 || habit.CategoryId == categoryId) && 
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

        public void NewSettings()
        {
            Settings = new Settings() { 
                ShowSavedSettings = Settings.ShowSavedSettings,
                SelectedCategoryId = Settings.SelectedCategoryId,
                ShowPercentMin = Settings.ShowPercentMin,
                ShowPinned = Settings.ShowPinned,
                ShowStarred = Settings.ShowStarred,
                ShowTwoMinute = Settings.ShowTwoMinute,
                ShowNeverDone = Settings.ShowNeverDone,
                ShowDoneOnce = Settings.ShowDoneOnce,
                ShowRatioOverPercentMin = Settings.ShowRatioOverPercentMin,
                ShowHelp = Settings.ShowHelp,
                ShowFilters = Settings.ShowFilters,
                ShowAdvancedFilters = Settings.ShowAdvancedFilters,
                ShowHabitId = Settings.ShowHabitId,
                ShowHabitIdUpDownButtons = Settings.ShowHabitIdUpDownButtons,
                ShowAllSelectOptions = Settings.ShowAllSelectOptions,
                ShowCategories = Settings.ShowCategories,
                ShowCategoriesInHeader = Settings.ShowCategoriesInHeader,
                ShowSearch = Settings.ShowSearch,
                ShowDateFilter = Settings.ShowDateFilter,
                ShowSort = Settings.ShowSort,
                ShowPinStar2min = Settings.ShowPinStar2min,
                ShowNotes = Settings.ShowNotes,
                ShowAverageInterval = Settings.ShowAverageInterval,
                ShowDesiredInterval = Settings.ShowDesiredInterval,
                ShowRatio = Settings.ShowRatio,
                ShowRatioOptions = Settings.ShowRatioOptions,
                ShowTimes = Settings.ShowTimes,
                BackupToGoogleDrive = Settings.BackupToGoogleDrive,
                Size = Settings.Size,
                Theme = Settings.Theme,
                Ratio = Settings.Ratio,
                Sort = Settings.Sort,
            };

            if (SettingsList.Any())
            {
                //SettingsId = SettingsList.Last().Id + 1;
                Settings.SelectedSettingsId = SettingsList.Last().Id + 1;
            }
        }

        public async Task SaveSettings()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (Settings.Id == 0)
            {
                db.Settings.Add(Settings);

                Settings settings = db.Settings.First();
                settings.SelectedSettingsId = SettingsId;

                await db.SaveChanges();

                await LoadData();
            }
            else if (db.Settings.SingleOrDefault(s => s.Id == Settings.Id) is Settings dbSettings)
            {
                dbSettings.Description = Settings.Description;
                dbSettings.SelectedSettingsId = Settings.SelectedSettingsId;
                dbSettings.ShowSavedSettings = Settings.ShowSavedSettings;
                dbSettings.SelectedCategoryId = Settings.SelectedCategoryId;
                dbSettings.ShowPercentMin = Settings.ShowPercentMin;
                dbSettings.ShowPinned = Settings.ShowPinned;
                dbSettings.ShowStarred = Settings.ShowStarred;
                dbSettings.ShowTwoMinute = Settings.ShowTwoMinute;
                dbSettings.ShowNeverDone = Settings.ShowNeverDone;
                dbSettings.ShowDoneOnce = Settings.ShowDoneOnce;
                dbSettings.ShowRatioOverPercentMin = Settings.ShowRatioOverPercentMin;
                dbSettings.ShowHelp = Settings.ShowHelp;
                dbSettings.ShowFilters = Settings.ShowFilters;
                dbSettings.ShowAdvancedFilters = Settings.ShowAdvancedFilters;
                dbSettings.ShowHabitId = Settings.ShowHabitId;
                dbSettings.ShowHabitIdUpDownButtons = Settings.ShowHabitIdUpDownButtons;
                dbSettings.ShowAllSelectOptions = Settings.ShowAllSelectOptions;
                dbSettings.ShowCategories = Settings.ShowCategories;
                dbSettings.ShowCategoriesInHeader = Settings.ShowCategoriesInHeader;
                dbSettings.ShowSearch = Settings.ShowSearch;
                dbSettings.ShowDateFilter = Settings.ShowDateFilter;
                dbSettings.ShowSort = Settings.ShowSort;
                dbSettings.ShowPinStar2min = Settings.ShowPinStar2min;
                dbSettings.ShowNotes = Settings.ShowNotes;
                dbSettings.ShowAverageInterval = Settings.ShowAverageInterval;
                dbSettings.ShowDesiredInterval = Settings.ShowDesiredInterval;
                dbSettings.ShowRatio = Settings.ShowRatio;
                dbSettings.ShowRatioOptions = Settings.ShowRatioOptions;
                dbSettings.ShowTimes = Settings.ShowTimes;
                dbSettings.BackupToGoogleDrive = Settings.BackupToGoogleDrive;
                dbSettings.Size = Settings.Size;
                dbSettings.Theme = Settings.Theme;
                dbSettings.Ratio = Settings.Ratio;
                dbSettings.Sort = Settings.Sort;

                if (Settings.Id != 1)
                {
                    Settings settings = db.Settings.First();
                    settings.SelectedSettingsId = SettingsId;
                }

                await db.SaveChanges();
            }

            OnPropertyChanged(nameof(Settings));
        }

        public async Task DeleteSettings(Settings settings)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            db.Settings.Remove(settings);

            await db.SaveChanges();

            await LoadData();
        }

        public async Task LoadData()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            bool save = false;

            if (db.Settings.Count == 0)
            {
                Dimensions dimensions = await JsInterop.GetDimensions();

                if (dimensions.Width < 576)
                    db.Settings.Add(new Settings() { Id = 1, Description = "Main", SelectedSettingsId = 1, Size = "small", Theme = "lumen" });
                else
                    db.Settings.Add(new Settings() { Id = 1, Description = "Main", SelectedSettingsId = 1, Size = "medium", Theme = "superhero" });

                save = true;
            }

            Settings = db.Settings.First();

            if (string.IsNullOrEmpty(Settings.Description))
            {
                Settings.Description = "Main";
                save = true;
            }

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
            SettingsList = db.Settings.ToList();
            TimeList = db.Times.ToList();

            CategoryDict = CategoryList.ToDictionary(category => category.Id);
            HabitDict = HabitList.ToDictionary(habit => habit.Id);
            SettingsDict = SettingsList.ToDictionary(settings => settings.Id);
            TimeDict = TimeList.ToDictionary(time => time.Id);

            if (SettingsDict.ContainsKey(SettingsId))
            {
                Settings = SettingsDict[SettingsId];
            }

            foreach (Time time in TimeList)
            {
                if (HabitDict.ContainsKey(time.HabitId))
                    HabitDict[time.HabitId].TimeList.Add(time);
            }

            foreach (Habit habit in HabitList)
            {
                if (habit.TimeList.Count > 1)
                    habit.AverageInterval = TimeSpan.FromMilliseconds(habit.TimeList.Zip(habit.TimeList.Skip(1), (x, y) => (y.DateTime - x.DateTime).TotalMilliseconds).Average());

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

            /*

            bool changed = false;

            HabitList = db.Habits.ToList();

            do
            {
                changed = false;

                foreach (Habit habit in HabitList)
                {
                    if (habit.TimeList.Count > 1)
                    {
                        Time lastTime = habit.TimeList.Last();

                        DateTime dateTime = lastTime.DateTime + habit.AverageInterval;

                        if (lastTime.DateTime + habit.AverageInterval < DateTime.Now)
                        {
                            Time item = new Time { HabitId = habit.Id, DateTime = lastTime.DateTime + (0.98 * habit.AverageInterval) };

                            habit.TimeList.Add(item);

                            db.Times.Add(item);

                            changed = true;
                        }
                    }
                }
            } while (changed);

            /**/

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
                dbHabit.Notes = habit.Notes;
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

            db.Habits.Add(new Habit() { Id = 1, CategoryId = 2, Description = "Drink a glass of water", DesiredInterval = new TimeSpan(0, 8, 0, 0), IsStarred = true, IsTwoMinute = true });
            db.Habits.Add(new Habit() { Id = 2, CategoryId = 2, Description = "Eat a piece of fruit", DesiredInterval = new TimeSpan(0, 12, 0, 0), IsTwoMinute = true });

            db.Habits.Add(new Habit() { Id = 3, CategoryId = 3, Description = "Stretch & workout", IsStarred = true });
            db.Habits.Add(new Habit() { Id = 4, CategoryId = 3, Description = "Go hiking", DesiredInterval = new TimeSpan(7, 0, 0, 0) });

            db.Habits.Add(new Habit() { Id = 5, CategoryId = 4, Description = "Go to a hairdresser", DesiredInterval = new TimeSpan(21, 0, 0, 0) });
            db.Habits.Add(new Habit() { Id = 6, CategoryId = 4, Description = "Buy new clothes", DesiredInterval = new TimeSpan(56, 0, 0, 0) });

            db.Habits.Add(new Habit() { Id = 7, CategoryId = 5, Description = "Take a walk" });
            db.Habits.Add(new Habit() { Id = 8, CategoryId = 5, Description = "Meditate", IsStarred = true });

            db.Habits.Add(new Habit() { Id = 9, CategoryId = 6, Description = "Call parents", IsStarred = true });
            db.Habits.Add(new Habit() { Id = 10, CategoryId = 6, Description = "Do someone a favor", DesiredInterval = new TimeSpan(14, 0, 0, 0) });

            db.Habits.Add(new Habit() { Id = 11, CategoryId = 7, Description = "Read a book", IsStarred = true });
            db.Habits.Add(new Habit() { Id = 12, CategoryId = 7, Description = "Get a massage", DesiredInterval = new TimeSpan(28, 0, 0, 0), IsStarred = true, IsPinned = true });

            db.Habits.Add(new Habit() { Id = 13, CategoryId = 8, Description = "Learn Spanish" });
            db.Habits.Add(new Habit() { Id = 14, CategoryId = 8, Description = "Play the piano" });

            db.Habits.Add(new Habit() { Id = 15, CategoryId = 9, Description = "Clean dust under the bed", DesiredInterval = new TimeSpan(14, 0, 0, 0) });
            db.Habits.Add(new Habit() { Id = 16, CategoryId = 9, Description = "Clean the windows", DesiredInterval = new TimeSpan(56, 0, 0, 0) });

            db.Habits.Add(new Habit() { Id = 17, CategoryId = 10, Description = "Ask for a raise", DesiredInterval = new TimeSpan(182, 0, 0, 0), IsPinned = true });
            db.Habits.Add(new Habit() { Id = 18, CategoryId = 10, Description = "Take a break", DesiredInterval = new TimeSpan(0, 8, 0, 0), IsTwoMinute = true });

            db.Times.Add(new Time() { Id = 1, HabitId = 5, DateTime = DateTime.Now.AddDays(-50) });
            db.Times.Add(new Time() { Id = 2, HabitId = 5, DateTime = DateTime.Now.AddDays(-28) });

            db.Times.Add(new Time() { Id = 3, HabitId = 12, DateTime = DateTime.Now.AddDays(-28) });
            db.Times.Add(new Time() { Id = 4, HabitId = 12, DateTime = DateTime.Now.AddDays(-7) });

            db.Times.Add(new Time() { Id = 5, HabitId = 15, DateTime = DateTime.Now.AddDays(-27) });
            db.Times.Add(new Time() { Id = 6, HabitId = 15, DateTime = DateTime.Now.AddDays(-13) });

            db.Times.Add(new Time() { Id = 7, HabitId = 16, DateTime = DateTime.Now.AddDays(-70) });
            db.Times.Add(new Time() { Id = 8, HabitId = 16, DateTime = DateTime.Now.AddDays(-12) });

            db.Times.Add(new Time() { Id = 9, HabitId = 17, DateTime = DateTime.Now.AddDays(-300) });

            db.Settings.Add(new Settings() { Description = "Hide", ShowSavedSettings = true, Size = Settings.Size, Theme = Settings.Theme, ShowHelp = false, ShowSearch = false, ShowAverageInterval = false, ShowRatio = false });
            db.Settings.Add(new Settings() { Description = "Show", ShowSavedSettings = true, Size = Settings.Size, Theme = Settings.Theme, ShowHelp = false,
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

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }
    }
}
