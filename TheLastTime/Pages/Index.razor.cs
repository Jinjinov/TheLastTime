using Blazorise;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
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

        readonly Dictionary<string, ButtonSize> ButtonSizeDict = new Dictionary<string, ButtonSize>()
        {
            { "small", ButtonSize.Small },
            { "medium", ButtonSize.None },
            { "large", ButtonSize.Large }
        };

        ButtonSize ButtonSize => ButtonSizeDict[DataService.Settings.Size];

        readonly Dictionary<string, Size> SizeDict = new Dictionary<string, Size>()
        {
            { "small", Size.Small },
            { "medium", Size.None },
            { "large", Size.Large }
        };

        Size Size => SizeDict[DataService.Settings.Size];
    }
}
