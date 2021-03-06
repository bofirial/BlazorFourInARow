using Blazor.Extensions.Logging;
using Blazor.Extensions.Storage;
using BlazorFourInARow.BusinessLogic;
using BlazorFourInARow.Common.Validators;
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
            services.AddSingleton<ICurrentGameStateProvider, CurrentGameStateProvider>();
            services.AddSingleton<IGamePieceDropper, GamePieceDropper>();
            services.AddSingleton<ISignalRConnectionFactory, SignalRConnectionFactory>();
            services.AddSingleton<IGameJoiner, GameJoiner>();

            services.AddSingleton<ApplicationReseter, ApplicationReseter>();

            services.AddSingleton<IGameStateManager, GameStateManager>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
