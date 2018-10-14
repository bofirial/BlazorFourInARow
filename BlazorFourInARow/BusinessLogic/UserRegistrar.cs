using BlazorFourInARow.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace BlazorFourInARow.BusinessLogic
{
    public class UserRegistrar : IUserRegistrar
    {
        private readonly IUserConnectionInfoStore _currentUserStore;
        private readonly ILogger<UserRegistrar> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;

        public UserRegistrar(IUserConnectionInfoStore currentUserStore, ILogger<UserRegistrar> logger,
            HttpClient httpClient, IServiceBaseUrlProvider serviceBaseUrlProvider)
        {
            _currentUserStore = currentUserStore;
            _logger = logger;
            _httpClient = httpClient;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
        }

        public async Task RegisterUserAsync(User user)
        {
            user.UserId = Guid.NewGuid().ToString();

            _logger.LogInformation($"Registering user ({user.UserId}) on the server.");

            var userConnectionInfo =
                await _httpClient.PostJsonAsync<UserConnectionInfo>(
                    $"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/user", user);

            await _currentUserStore.SetUserConnectionInfoAsync(userConnectionInfo);
        }
    }
}
