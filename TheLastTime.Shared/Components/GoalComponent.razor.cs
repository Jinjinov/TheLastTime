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

        readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        protected override void OnInitialized()
        {
            MarkdownToHtml();

            base.OnInitialized();
        }

        private void MarkdownToHtml()
        {
            markdownHtml = Markdown.ToHtml(Goal.Text, pipeline);
        }

        async Task OnTitleChanged(string value)
        {
            Goal.Title = value;

            await DataService.Save(Goal);
        }

        async Task OnMarkdownValueChanged(string value)
        {
            Goal.Text = value;

            MarkdownToHtml();

            await DataService.Save(Goal);
        }
    }
}
