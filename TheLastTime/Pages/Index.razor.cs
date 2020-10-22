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

        Dictionary<string, string> bootswatchThemeDict = new Dictionary<string, string>()
        {
            { "cerulean", "sha384-3fdgwJw17Bi87e1QQ4fsLn4rUFqWw//KU0g8TvV6quvahISRewev6/EocKNuJmEw" },
            { "cosmo", "sha384-5QFXyVb+lrCzdN228VS3HmzpiE7ZVwLQtkt+0d9W43LQMzz4HBnnqvVxKg6O+04d" },
            { "cyborg", "sha384-nEnU7Ae+3lD52AK+RGNzgieBWMnEfgTbRHIwEvp1XXPdqdO6uLTd/NwXbzboqjc2" },
            { "darkly", "sha384-nNK9n28pDUDDgIiIqZ/MiyO3F4/9vsMtReZK39klb/MtkZI3/LtjSjlmyVPS3KdN" },
            { "flatly", "sha384-qF/QmIAj5ZaYFAeQcrQ6bfVMAh4zZlrGwTPY7T/M+iTTLJqJBJjwwnsE5Y0mV7QK" },
            { "journal", "sha384-QDSPDoVOoSWz2ypaRUidLmLYl4RyoBWI44iA5agn6jHegBxZkNqgm2eHb6yZ5bYs" },
            { "litera", "sha384-enpDwFISL6M3ZGZ50Tjo8m65q06uLVnyvkFO3rsoW0UC15ATBFz3QEhr3hmxpYsn" },
            { "lumen", "sha384-GzaBcW6yPIfhF+6VpKMjxbTx6tvR/yRd/yJub90CqoIn2Tz4rRXlSpTFYMKHCifX" },
            { "lux", "sha384-9+PGKSqjRdkeAU7Eu4nkJU8RFaH8ace8HGXnkiKMP9I9Te0GJ4/km3L1Z8tXigpG" },
            { "materia", "sha384-B4morbeopVCSpzeC1c4nyV0d0cqvlSAfyXVfrPJa25im5p+yEN/YmhlgQP/OyMZD" },
            { "minty", "sha384-H4X+4tKc7b8s4GoMrylmy2ssQYpDHoqzPa9aKXbDwPoPUA3Ra8PA5dGzijN+ePnH" },
            { "pulse", "sha384-L7+YG8QLqGvxQGffJ6utDKFwmGwtLcCjtwvonVZR/Ba2VzhpMwBz51GaXnUsuYbj" },
            { "sandstone", "sha384-zEpdAL7W11eTKeoBJK1g79kgl9qjP7g84KfK3AZsuonx38n8ad+f5ZgXtoSDxPOh" },
            { "simplex", "sha384-FYrl2Nk72fpV6+l3Bymt1zZhnQFK75ipDqPXK0sOR0f/zeOSZ45/tKlsKucQyjSp" },
            { "sketchy", "sha384-RxqHG2ilm4r6aFRpGmBbGTjsqwfqHOKy1ArsMhHusnRO47jcGqpIQqlQK/kmGy9R" },
            { "slate", "sha384-8iuq0iaMHpnH2vSyvZMSIqQuUnQA7QM+f6srIdlgBrTSEyd//AWNMyEaSF2yPzNQ" },
            { "solar", "sha384-NCwXci5f5ZqlDw+m7FwZSAwboa0svoPPylIW3Nf+GBDsyVum+yArYnaFLE9UDzLd" },
            { "spacelab", "sha384-F1AY0h4TrtJ8OCUQYOzhcFzUTxSOxuaaJ4BeagvyQL8N9mE4hrXjdDsNx249NpEc" },
            { "superhero", "sha384-HnTY+mLT0stQlOwD3wcAzSVAZbrBp141qwfR4WfTqVQKSgmcgzk+oP0ieIyrxiFO" },
            { "united", "sha384-JW3PJkbqVWtBhuV/gsuyVVt3m/ecRJjwXC3gCXlTzZZV+zIEEl6AnryAriT7GWYm" },
            { "yeti", "sha384-mLBxp+1RMvmQmXOjBzRjqqr0dP9VHU2tb3FK6VB0fJN/AOu7/y+CAeYeWJZ4b3ii" },
        };

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
