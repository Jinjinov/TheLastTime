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

        protected override void OnInitialized()
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
            markdownHtml = Markdown.ToHtml(Note.Text, pipeline);

            base.OnInitialized();
        }

        async Task OnTitleChanged(string value)
        {
            Note.Title = value;

            await DataService.Save(Note);
        }

        async Task OnMarkdownValueChanged(string value)
        {
            Note.Text = value;

            markdownHtml = Markdig.Markdown.ToHtml(Note.Text);

            await DataService.Save(Note);
        }
    }
}
