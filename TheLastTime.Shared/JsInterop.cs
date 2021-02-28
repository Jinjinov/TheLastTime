using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TheLastTime.Shared
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class Dimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class JsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public JsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/TheLastTime.Shared/jsInterop.js").AsTask());
        }

        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async Task<Dimensions> GetDimensions()
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<Dimensions>("getDimensions");
        }

        public async ValueTask SetElementProperty(ElementReference element, string property, object value)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("setElementProperty", element, property, value);
        }

        public async Task SaveAsUTF8(string filename, string content)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);

            var module = await moduleTask.Value;
            await module.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
