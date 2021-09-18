using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public partial class NotesComponent
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        async Task NewNote()
        {
            Note note = new Note();

            await DataService.Save(note);
        }
    }
}
