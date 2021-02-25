using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        private ElementReference inputElement;

        string? elementId;
        public string ElementId
        {
            get
            {
                // generate ID only on first use
                if (elementId == null)
                    elementId = Blazorise.Utils.IDGenerator.Instance.Generate;

                return elementId;
            }
            private set
            {
                elementId = value;
            }
        }

        private async Task OnChange(ChangeEventArgs e)
        {
            if (Checked == false)
                Checked = true;
            else if(Checked == true)
                Checked = null;
            else if (Checked == null)
                Checked = false;

            isChecked = Checked != false;

            bool isIndeterminate = Checked == null;

            await jsRuntime.InvokeVoidAsync("setElementProperty", inputElement, "indeterminate", isIndeterminate);

            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}
