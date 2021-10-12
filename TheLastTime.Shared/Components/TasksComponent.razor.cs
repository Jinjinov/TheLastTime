using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Components
{
    public partial class TasksComponent
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        async Task NewTask()
        {
            Tasky task = new Tasky();

            await DataService.Save(task);
        }
    }
}
