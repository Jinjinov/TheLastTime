using Blazor.IndexedDB.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TheLastTime.Data
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

        public Settings Settings = new Settings();

        public List<Category> CategoryList { get; set; } = new List<Category>();
        public List<Habit> HabitList { get; set; } = new List<Habit>();
        public List<Time> TimeList { get; set; } = new List<Time>();

        public Dictionary<long, Category> CategoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Habit> HabitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Time> TimeDict { get; set; } = new Dictionary<long, Time>();

        readonly IIndexedDbFactory DbFactory;

        public DataService(IIndexedDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public async Task SaveSettings()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            Settings settings = db.Settings.Single();

            settings.Size = Settings.Size;
            settings.Theme = Settings.Theme;

            await db.SaveChanges();

            OnPropertyChanged(nameof(Settings));
        }

        public async Task LoadData()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            bool save = false;

            if (db.Settings.Count == 0)
            {
                db.Settings.Add(new Settings() { Size= "medium", Theme = "superhero" });
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
                if (CategoryDict.ContainsKey(habit.CategoryId))
                    CategoryDict[habit.CategoryId].HabitList.Add(habit);
            }
        }

        public async Task ClearData()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            foreach (Category category in db.Categories)
            {
                if (category.Id != 1)
                    db.Categories.Remove(category);
            }

            db.Habits.Clear();
            db.Times.Clear();

            await db.SaveChanges();

            await LoadData();
        }

        public async Task AddData(List<Category> categoryList)
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

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
        }

        public async Task SaveCategory(Category category)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
                dbHabit.Priority = habit.Priority;
                dbHabit.DesiredInterval = habit.DesiredInterval;
            }

            await db.SaveChanges();

            await LoadData();
        }

        public async Task DeleteHabit(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
        }

        public async Task DeleteTime(Time time)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Times.Remove(time);
            await db.SaveChanges();

            await LoadData();
        }

        public async Task SeedExamples()
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

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
