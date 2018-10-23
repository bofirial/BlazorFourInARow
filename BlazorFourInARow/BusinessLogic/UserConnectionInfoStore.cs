using Blazor.Extensions.Storage;
using BlazorFourInARow.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace BlazorFourInARow.BusinessLogic
{
    public class UserConnectionInfoStore : IUserConnectionInfoStore
    {
        private readonly LocalStorage _localStorage;
        private readonly ILogger<UserConnectionInfoStore> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;
        const string KEY = "user-connection-info";

        public UserConnectionInfoStore(LocalStorage localStorage, ILogger<UserConnectionInfoStore> logger, HttpClient httpClient, IServiceBaseUrlProvider serviceBaseUrlProvider)
        {
            _localStorage = localStorage;
            _logger = logger;
            _httpClient = httpClient;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
        }

        public async Task<UserConnectionInfo> GetUserConnectionInfoAsync()
        {
            var userConnectionInfo = await _localStorage.GetItem<UserConnectionInfo>(KEY);

            if (userConnectionInfo?.User != null)
            {
                userConnectionInfo =
                    await _httpClient.PostJsonAsync<UserConnectionInfo>(
                        $"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/user", userConnectionInfo.User); 
            }

            _logger.LogInformation($"User Object Information.  IsNull = {null == userConnectionInfo}.  ID = {userConnectionInfo?.User?.UserId}");

            return userConnectionInfo;
        }

        public async Task SetUserConnectionInfoAsync(UserConnectionInfo userConnectionInfo)
        {
            _logger.LogInformation($"Setting current user to: {userConnectionInfo?.User?.DisplayName}.");

            await _localStorage.SetItem(KEY, userConnectionInfo);

            OnCurrentUserChanged(userConnectionInfo);
        }

        public event Action<UserConnectionInfo> UserConnectionInfoChanged;

        protected virtual void OnCurrentUserChanged(UserConnectionInfo userConnectionInfo)
        {
            UserConnectionInfoChanged?.Invoke(userConnectionInfo);
        }
    }
}
