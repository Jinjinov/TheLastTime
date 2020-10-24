using Blazor.IndexedDB.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    public class DataService
    {
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
        }

        public async Task LoadData()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            bool save = false;

            if (db.Settings.Count == 0)
            {
                db.Settings.Add(new Settings() { Theme = "superhero" });
                save = true;
            }

            Settings = db.Settings.Single();

            if (db.Categories.Count == 0)
            {
                db.Categories.Add(new Category() { Description = "No category" });
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

            //db.Categories.Clear();
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
            }

            await db.SaveChanges();

            await LoadData();
        }

        public async Task DeleteCategory(Category category)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            foreach (Habit habit in category.HabitList)
            {
                habit.CategoryId = 1;
            }

            db.Categories.Remove(category);

            await db.SaveChanges();

            await LoadData();
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
    }
}
