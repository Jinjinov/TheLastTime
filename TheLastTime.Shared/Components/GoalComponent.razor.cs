using Markdig;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
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

        protected override void OnInitialized()
        {
            MarkdownToHtml();

            base.OnInitialized();
        }

        private void MarkdownToHtml()
        {
            markdownHtml = Markdown.ToHtml(Goal.Notes, pipeline);
        }

        async Task OnDescriptionChanged(string value)
        {
            Goal.Description = value;

            await DataService.Save(Goal);
        }

        async Task OnMarkdownValueChanged(string value)
        {
            Goal.Notes = value;

            MarkdownToHtml();

            await DataService.Save(Goal);
        }
    }
}
