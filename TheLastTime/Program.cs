using Blazor.IndexedDB.Framework;
//using Blazorise;
//using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheLastTime
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            //builder.Services
            //    .AddBlazorise(options =>
            //    {
            //        options.ChangeTextOnKeyPress = true;
            //    })
            //    .AddBootstrapProviders();

            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            var host = builder.Build();

            //host.Services
            //    .UseBootstrapProviders();

            await host.RunAsync();
        }
    }
}
