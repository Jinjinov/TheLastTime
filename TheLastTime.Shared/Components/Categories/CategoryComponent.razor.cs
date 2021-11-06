using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components.Categories
{
    public sealed partial class CategoryComponent : IDisposable
    {
        public bool editCategory;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        protected override void OnInitialized()
        {
            DataService.PropertyChanged += PropertyChanged;
            State.PropertyChanged += PropertyChanged;
        }

        void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= PropertyChanged;
            State.PropertyChanged -= PropertyChanged;
        }

        async Task SaveCategory()
        {
            // TODO: sync DataService list with db list
            await DataService.SaveCategory(State.SelectedCategory);

            editCategory = false;
        }
    }
}
