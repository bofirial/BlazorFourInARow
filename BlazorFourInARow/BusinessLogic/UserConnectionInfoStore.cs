using Blazor.Extensions.Storage;
using BlazorFourInARow.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlazorFourInARow.BusinessLogic
{
    public class UserConnectionInfoStore : IUserConnectionInfoStore
    {
        private readonly LocalStorage _localStorage;
        private readonly ILogger<UserConnectionInfoStore> _logger;
        const string KEY = "user-connection-info";

        public UserConnectionInfoStore(LocalStorage localStorage, ILogger<UserConnectionInfoStore> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }

        public async Task<UserConnectionInfo> GetUserConnectionInfoAsync()
        {
            var userConnectionInfo = await _localStorage.GetItem<UserConnectionInfo>(KEY);

            _logger.LogDebug($"User Object Information.  IsNull = {null == userConnectionInfo}.  ID = {userConnectionInfo?.User?.UserId}");

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
