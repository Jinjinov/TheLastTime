using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IndexedDB.Blazor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared
{
    public static class Services
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            serviceCollection.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            serviceCollection.AddScoped<IDatabaseAccess, DatabaseAccess>();

            serviceCollection.AddScoped<JsInterop>();

            serviceCollection.AddScoped<DataService>();

            serviceCollection.AddScoped<GoogleDrive>();

            serviceCollection.AddScoped<State>();

            serviceCollection.AddScoped<ThemeOptions>();

            return serviceCollection;
        }

        public static async Task UseServices(this IServiceProvider serviceProvider)
        {
            //serviceProvider.UseBootstrapProviders().UseFontAwesomeIcons(); // v0.9.2 ---> v0.9.3

            await serviceProvider.GetRequiredService<DataService>().LoadData();

            serviceProvider.GetRequiredService<GoogleDrive>(); // make sure that there is one instance of GoogleDrive
        }
    }
}
