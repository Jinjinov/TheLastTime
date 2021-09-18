using Markdig;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public partial class NoteComponent
    {
        [Parameter]
        public Note Note { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        string markdownHtml = string.Empty;

        readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();

        protected override void OnInitialized()
        {
            MarkdownToHtml();

            base.OnInitialized();
        }

        private void MarkdownToHtml()
        {
            markdownHtml = Markdown.ToHtml(Note.Text, pipeline);
        }

        async Task OnTitleChanged(string value)
        {
            Note.Title = value;

            await DataService.Save(Note);
        }

        async Task OnMarkdownValueChanged(string value)
        {
            Note.Text = value;

            MarkdownToHtml();

            await DataService.Save(Note);
        }
    }
}
