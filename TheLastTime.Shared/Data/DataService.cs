using Markdig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public class DataService : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

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

        public Category RootCategory { get; set; } = new Category();

        public List<Category> CategoryList { get; set; } = new List<Category>();
        public List<Habit> HabitList { get; set; } = new List<Habit>();
        public List<Goal> GoalList { get; set; } = new List<Goal>();
        public List<Settings> SettingsList { get; set; } = new List<Settings>();
        public List<Time> TimeList { get; set; } = new List<Time>();
        public List<Tasky> TaskList { get; set; } = new List<Tasky>();

        public Dictionary<long, Category> CategoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Habit> HabitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Goal> GoalDict { get; set; } = new Dictionary<long, Goal>();
        public Dictionary<long, Settings> SettingsDict { get; set; } = new Dictionary<long, Settings>();
        public Dictionary<long, Time> TimeDict { get; set; } = new Dictionary<long, Time>();
        public Dictionary<long, Tasky> TaskDict { get; set; } = new Dictionary<long, Tasky>();

        readonly JsInterop JsInterop;
        public readonly IDatabaseAccess DatabaseAccess;

        public DataService(JsInterop jsInterop, IDatabaseAccess databaseAccess)
        {
            JsInterop = jsInterop;
            DatabaseAccess = databaseAccess;
        }

        readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

        public string MarkdownToHtml(string markdown) => Markdown.ToHtml(markdown, markdownPipeline);

        public void NewSettings()
        {
            Settings settings = new Settings();

            Settings.CopyTo(settings);

            settings.Description = string.Empty;

            settings.SelectedSettingsId = SettingsList.Any() ? SettingsList.Last().Id + 1 : 0;

            Settings = settings;
        }

        public async Task SaveSettings()
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (db.Settings.SingleOrDefault(s => s.Id == Settings.Id) is Settings dbSettings)
            {
                Settings.CopyTo(dbSettings);

                if (Settings.Id != 1)
                {
                    Settings settings = db.Settings.First();
                    settings.SelectedSettingsId = SettingsId;
                }

                await db.SaveChanges();
            }
            else
            {
                Settings.Id = db.Settings.Max(s => s.Id) + 1;

                db.Settings.Add(Settings);

                Settings settings = db.Settings.First();
                settings.SelectedSettingsId = SettingsId;

                await db.SaveChanges();

                await LoadData();
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
                RootCategory = new Category() { Id = 1, Description = "Main" };
                db.Categories.Add(RootCategory);
                save = true;
            }

            if (save)
            {
                await db.SaveChanges();
            }

            CategoryList = db.Categories.ToList();
            HabitList = db.Habits.ToList();
            GoalList = db.Goals.ToList();
            SettingsList = db.Settings.ToList();
            TimeList = db.Times.ToList();
            TaskList = db.Tasks.ToList();

            CategoryDict = CategoryList.ToDictionary(category => category.Id);
            HabitDict = HabitList.ToDictionary(habit => habit.Id);
            GoalDict = GoalList.ToDictionary(note => note.Id);
            SettingsDict = SettingsList.ToDictionary(settings => settings.Id);
            TimeDict = TimeList.ToDictionary(time => time.Id);
            TaskDict = TaskList.ToDictionary(task => task.Id);

            RootCategory = CategoryList.First();

            foreach (Category category in CategoryList)
            {
                if (CategoryDict.ContainsKey(category.CategoryId))
                {
                    Category parent = CategoryDict[category.CategoryId];

                    if (parent.CategoryList == null)
                        parent.CategoryList = new List<Category>();

                    parent.CategoryList.Add(category);
                }
            }

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

                habit.NotesMarkdownHtml = MarkdownToHtml(string.IsNullOrEmpty(habit.Notes) ? "Edit habit to add notes" : habit.Notes);

                if (CategoryDict.ContainsKey(habit.CategoryId))
                    CategoryDict[habit.CategoryId].HabitList.Add(habit);
            }

            foreach (Goal goal in GoalList)
            {
                goal.NotesMarkdownHtml = MarkdownToHtml(string.IsNullOrEmpty(goal.Notes) ? "Edit goal to add notes" : goal.Notes);

                if (CategoryDict.ContainsKey(goal.CategoryId))
                    CategoryDict[goal.CategoryId].GoalList.Add(goal);
            }

            foreach (Tasky task in TaskList)
            {
                task.NotesMarkdownHtml = MarkdownToHtml(string.IsNullOrEmpty(task.Notes) ? "Edit task to add notes" : task.Notes);

                if (CategoryDict.ContainsKey(task.CategoryId))
                    CategoryDict[task.CategoryId].TaskList.Add(task);
            }
        }

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

        public async Task AddGoals(List<Goal> goalList)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            foreach (Goal goal in goalList)
            {
                db.Goals.Add(goal);
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(GoalList));
        }

        public async Task AddCategories(List<Category> categoryList)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            // TODO: recurse this loop with category.CategoryList:

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

        public async Task GetCategories(JsonElement json)
        {
            JsonElement jsonName = json.GetProperty("name");
            JsonElement jsonNodes = json.GetProperty("nodes");

            string name = jsonName.GetString() ?? string.Empty;

            using IDatabase db = await DatabaseAccess.CreateDatabase();

            Category? root = CategoryList.FirstOrDefault(c => c.Description == name);

            if (root == null)
            {
                long maxId = CategoryList.Max(category => category.Id);

                root = new Category
                {
                    Id = ++maxId,
                    CategoryId = RootCategory.Id,
                    Description = name
                };

                db.Categories.Add(root);

                CategoryList.Add(root);
            }

            TraverseCategories(db, jsonNodes, root);

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }

        private void TraverseCategories(IDatabase db, JsonElement json, Category parent)
        {
            foreach (JsonElement jsonElement in json.EnumerateArray())
            {
                JsonElement jsonName = jsonElement.GetProperty("name");
                string name = jsonName.GetString() ?? string.Empty;

                if (jsonElement.TryGetProperty("text", out JsonElement jsonText))
                {
                    if (!name.EndsWith(".md"))
                        continue;

                    long maxId = GoalList.Any() ? GoalList.Max(g => g.Id) : 0;

                    Goal goal = new Goal
                    {
                        Id = ++maxId,
                        CategoryId = parent.Id,
                        Description = name[0..^3],
                        Notes = jsonText.GetString() ?? string.Empty
                    };

                    parent.GoalList.Add(goal);

                    db.Goals.Add(goal);

                    GoalList.Add(goal);
                }
                else if (jsonElement.TryGetProperty("nodes", out JsonElement jsonNodes))
                {
                    if (name.StartsWith('.'))
                        continue;

                    Category? category = CategoryList.FirstOrDefault(c => c.Description == name);

                    if (category == null)
                    {
                        long maxId = CategoryList.Max(category => category.Id);

                        category = new Category
                        {
                            Id = ++maxId,
                            CategoryId = parent.Id,
                            Description = name
                        };

                        db.Categories.Add(category);

                        CategoryList.Add(category);
                    }

                    if (parent.CategoryList == null)
                    {
                        parent.CategoryList = new List<Category>();
                    }

                    if (!parent.CategoryList.Any(c => c.Id == category.Id))
                    {
                        parent.CategoryList.Add(category);
                    }

                    TraverseCategories(db, jsonNodes, category);
                }
            }
        }

        public async Task Save<T>(T entity) where T : IEntity<T>
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (db.GetCollection<T>().SingleOrDefault(e => e.Id == entity.Id) is T dbEntity)
            {
                entity.CopyTo(dbEntity);
            }
            else
            {
                entity.Id = db.GetCollection<T>().Max(e => e.Id) + 1;

                db.GetCollection<T>().Add(entity);
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(T));
        }

        public async Task SaveCategory(Category category)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (db.Categories.SingleOrDefault(c => c.Id == category.Id) is Category dbCategory)
            {
                category.CopyTo(dbCategory);
            }
            else
            {
                category.Id = db.Categories.Max(c => c.Id) + 1;

                db.Categories.Add(category);
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

            if (db.Habits.SingleOrDefault(h => h.Id == habit.Id) is Habit dbHabit)
            {
                habit.CopyTo(dbHabit);
            }
            else
            {
                habit.Id = db.Habits.Max(h => h.Id) + 1;

                db.Habits.Add(habit);
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

        public async Task SaveTime(Time time)
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
            {
                time.CopyTo(dbTime);
            }
            else
            {
                time.Id = db.Times.Max(t => t.Id) + 1;

                db.Times.Add(time);
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

            await db.Seed(Settings.Size, Settings.Theme);

            await LoadData();

            OnPropertyChanged(nameof(CategoryList));
        }
    }
}
