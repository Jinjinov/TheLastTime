using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public List<Category> CategoryList { get; set; } = new List<Category>();
        public List<Group> GroupList { get; set; } = new List<Group>();
        public List<Habit> HabitList { get; set; } = new List<Habit>();
        public List<Note> NoteList { get; set; } = new List<Note>();
        public List<Settings> SettingsList { get; set; } = new List<Settings>();
        public List<Time> TimeList { get; set; } = new List<Time>();
        public List<ToDo> ToDoList { get; set; } = new List<ToDo>();

        public Dictionary<long, Category> CategoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Group> GroupDict { get; set; } = new Dictionary<long, Group>();
        public Dictionary<long, Habit> HabitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Note> NoteDict { get; set; } = new Dictionary<long, Note>();
        public Dictionary<long, Settings> SettingsDict { get; set; } = new Dictionary<long, Settings>();
        public Dictionary<long, Time> TimeDict { get; set; } = new Dictionary<long, Time>();
        public Dictionary<long, ToDo> ToDoDict { get; set; } = new Dictionary<long, ToDo>();

        readonly JsInterop JsInterop;
        public readonly IDatabaseAccess DatabaseAccess;

        public DataService(JsInterop jsInterop, IDatabaseAccess databaseAccess)
        {
            JsInterop = jsInterop;
            DatabaseAccess = databaseAccess;
        }

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
                Settings.CopyTo(dbSettings);

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
            GroupList = db.Groups.ToList();
            HabitList = db.Habits.ToList();
            NoteList = db.Notes.ToList();
            SettingsList = db.Settings.ToList();
            TimeList = db.Times.ToList();
            ToDoList = db.ToDos.ToList();

            CategoryDict = CategoryList.ToDictionary(category => category.Id);
            GroupDict = GroupList.ToDictionary(group => group.Id);
            HabitDict = HabitList.ToDictionary(habit => habit.Id);
            NoteDict = NoteList.ToDictionary(note => note.Id);
            SettingsDict = SettingsList.ToDictionary(settings => settings.Id);
            TimeDict = TimeList.ToDictionary(time => time.Id);
            ToDoDict = ToDoList.ToDictionary(todo => todo.Id);

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

        public async Task Save<T>(T entity) where T : IEntity<T>
        {
            using IDatabase db = await DatabaseAccess.CreateDatabase();

            if (entity.Id == 0)
            {
                db.GetCollection<T>().Add(entity);
            }
            else if (db.GetCollection<T>().SingleOrDefault(e => e.Id == entity.Id) is T dbEntity)
            {
                entity.CopyTo(dbEntity);
            }

            await db.SaveChanges();

            await LoadData();

            OnPropertyChanged(nameof(T));
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
                category.CopyTo(dbCategory);
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
                habit.CopyTo(dbHabit);
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

            if (time.Id == 0)
            {
                db.Times.Add(time);
            }
            else if (db.Times.SingleOrDefault(t => t.Id == time.Id) is Time dbTime)
            {
                time.CopyTo(dbTime);
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
