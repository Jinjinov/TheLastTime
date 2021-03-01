using IndexedDB.Blazor;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Data;
using TheLastTime.Shared;

namespace TheLastTime
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            builder.Services.AddScoped<IDatabaseAccess, DatabaseAccess>();

            builder.Services.AddScoped<JsInterop>();

            builder.Services.AddScoped<DataService>();

            builder.Services.AddScoped<State>();

            builder.Services.AddScoped<ThemeOptions>();

            WebAssemblyHost host = builder.Build();

            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            await host.Services.GetRequiredService<DataService>().LoadData();

            await host.RunAsync();
        }
    }
}
