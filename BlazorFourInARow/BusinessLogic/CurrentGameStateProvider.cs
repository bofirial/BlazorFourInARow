using System.Net.Http;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.Extensions.Logging;

namespace BlazorFourInARow.BusinessLogic
{
    public class CurrentGameStateProvider : ICurrentGameStateProvider
    {
        private readonly ILogger<UserConnectionInfoStore> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;

        public CurrentGameStateProvider(ILogger<UserConnectionInfoStore> logger,
            HttpClient httpClient, IServiceBaseUrlProvider serviceBaseUrlProvider)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
        }

        public async Task<GameState> GetCurrentGameStateAsync()
        {
            return await _httpClient.GetJsonAsync<GameState>($"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/game");
        }
    }
}