using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IndexedDB.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TheLastTime.Data;
using TheLastTime.Shared;
using TheLastTime.Shared.Data;

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

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Local", options.ProviderOptions);

                //options.ProviderOptions.Authority = "https://accounts.google.com/";
                //options.ProviderOptions.RedirectUri = "https://localhost:44336/authentication/login-callback";
                //options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:44336/authentication/logout-callback";
                //options.ProviderOptions.ClientId = "592595683573-67d1cms7i452752f1n7ik0altkohdcod.apps.googleusercontent.com";
                options.ProviderOptions.ResponseType = "id_token token";
                options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive");
            });

            builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            builder.Services.AddScoped<IDatabaseAccess, DatabaseAccess>();

            builder.Services.AddScoped<JsInterop>();

            builder.Services.AddScoped<GoogleDrive>();

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
