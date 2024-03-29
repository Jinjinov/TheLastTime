﻿using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components.Goals
{
    public partial class GoalComponent
    {
        [Parameter]
        public Goal Goal { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        private void OnDescriptionChanged(string value)
        {
            Goal.Description = value;
        }

        private void OnNotesChanged(string value)
        {
            Goal.Notes = value;

            Goal.NotesMarkdownHtml = DataService.MarkdownToHtml(Goal.Notes);
        }

        async Task SaveGoal()
        {
            // TODO: sync DataService list with db list
            await DataService.Save(Goal);

            State.EditNote = false;
        }
    }
}
