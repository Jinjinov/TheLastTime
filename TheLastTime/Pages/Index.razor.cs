using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public partial class Index
    {
        Category newCategory = new Category();
        Category selectedCategory = new Category();

        Habit newHabit = new Habit();
        List<Habit> habitList;
        List<Category> categoryList;

        protected override async Task OnInitializedAsync()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            categoryList = db.Categories.ToList();
            habitList = db.Habits.ToList();
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
    }
}
