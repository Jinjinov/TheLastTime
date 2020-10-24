using Microsoft.AspNetCore.Components;
using TheLastTime.Data;

namespace TheLastTime.Pages
{
    public partial class Index
    {
        protected Category selectedCategory = new Category();
        protected long selectedCategoryId;

        protected Habit? selectedHabit;

        protected Time? selectedTime;

        protected bool editCategory;
        protected bool editHabit;
        protected bool editTime;

        [Inject]
        DataService DataService { get; set; } = null!;
    }
}
