using Markdig;
using Microsoft.AspNetCore.Components;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
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

        string markdownHtml = string.Empty;

        readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

        //protected override void OnInitialized() // is called once
        //{
        //    MarkdownToHtml(); // must be called on every render

        //    base.OnInitialized();
        //}

        protected override void OnParametersSet() // is called on every render
        {
            MarkdownToHtml(); // must be called on every render

            base.OnParametersSet();
        }

        private void MarkdownToHtml()
        {
            markdownHtml = Markdown.ToHtml(Goal.Notes, pipeline);
        }

        private void OnDescriptionChanged(string value)
        {
            Goal.Description = value;
        }

        private void OnMarkdownValueChanged(string value)
        {
            Goal.Notes = value;

            MarkdownToHtml();
        }
    }
}
