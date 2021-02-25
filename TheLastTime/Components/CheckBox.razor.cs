using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace TheLastTime.Components
{
    public partial class CheckBox
    {
        [Inject]
        IJSRuntime jsRuntime { get; set; } = null!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public bool? Checked { get; set; }

        [Parameter]
        public EventCallback<bool?> CheckedChanged { get; set; }

        private bool isChecked;

        private ElementReference elementReference;

        private string elementId = Guid.NewGuid().ToString();

        private async Task OnChange(ChangeEventArgs e)
        {
            Checked = Checked switch
            {
                false => true,
                true => null,
                null => false,
            };

            isChecked = Checked != false;

            bool isIndeterminate = Checked == null;

            await jsRuntime.InvokeVoidAsync("setElementProperty", elementReference, "indeterminate", isIndeterminate);

            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}
