using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public partial class Index
    {
        //string bootswatchTheme = string.Empty;
        string bootswatchTheme = "slate";

        Category newCategory = new Category();
        Category selectedCategory = new Category();

        Habit newHabit = new Habit();
        Habit selectedHabit = null;

        List<Category> categoryList;
        List<Habit> habitList;
        List<Time> timeList;

        Dictionary<long, Category> categoryDict;
        Dictionary<long, Habit> habitDict;
        Dictionary<long, Time> timeDict;

        protected override async Task OnInitializedAsync()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            categoryList = db.Categories.ToList();
            habitList = db.Habits.ToList();
            timeList = db.Times.ToList();

            categoryDict = categoryList.ToDictionary(category => category.Id);
            habitDict = habitList.ToDictionary(habit => habit.Id);
            timeDict = timeList.ToDictionary(time => time.Id);

            foreach (Time time in timeList)
            {
                if (habitDict.ContainsKey(time.HabitId))
                    habitDict[time.HabitId].TimeList.Add(time);
            }

            foreach (Habit habit in habitList)
            {
                if (categoryDict.ContainsKey(habit.CategoryId))
                    categoryDict[habit.CategoryId].HabitList.Add(habit);
            }
        }

        async Task LoadHabitList()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            if (selectedCategory.Id == 0)
            {
                habitList = db.Habits.ToList();
            }
            else
            {
                habitList = db.Habits.Where(habit => habit.CategoryId == selectedCategory.Id).ToList();
            }

            StateHasChanged();
        }

        async Task SaveNewCategory()
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Categories.Add(newCategory);
            await db.SaveChanges();

            await OnInitializedAsync();

            newCategory = new Category();
        }

        async Task DeleteCategory(Category category)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Categories.Remove(category);
            await db.SaveChanges();

            await OnInitializedAsync();
        }

        async Task SaveNewHabit()
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            newHabit.CategoryId = selectedCategory.Id;
            db.Habits.Add(newHabit);

            await db.SaveChanges();

            await LoadHabitList();

            newHabit = new Habit();
        }

        async Task DeleteHabit(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Habits.Remove(habit);
            await db.SaveChanges();

            await LoadHabitList();
        }

        async Task DoneHabit(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            Time time = new Time
            {
                HabitId = habit.Id,
                DateTime = DateTime.Now
            };

            db.Times.Add(time);

            await db.SaveChanges();

            await OnInitializedAsync();
        }

        string text = string.Empty;

        async Task ImportFile(InputFileChangeEventArgs e)
        {
            Stream stream = e.File.OpenReadStream();

            using StreamReader streamReader = new StreamReader(stream);

            text = await streamReader.ReadToEndAsync();
        }

        [Inject]
        IJSRuntime jsRuntime { get; set; }

        async Task ExportFile()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            await SaveAs(jsRuntime, "HelloWorld.txt", bytes);
        }

        async Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }
    }
}
