using Blazor.Extensions.Logging;
using Blazor.Extensions.Storage;
using BlazorFourInARow.BusinessLogic;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorFourInARow
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStorage();

            services.AddLogging(builder => builder
                .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
                .SetMinimumLevel(LogLevel.Information));

            services.AddSingleton<IUserConnectionInfoStore, UserConnectionInfoStore>();
            services.AddSingleton<IUserRegistrar, UserRegistrar>();
            services.AddSingleton<IServiceBaseUrlProvider, ServiceBaseUrlProvider>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
