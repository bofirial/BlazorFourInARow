using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blazor.Extensions;
using BlazorFourInARow.Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlazorFourInARow.BusinessLogic
{
    public class SignalRConnectionFactory : ISignalRConnectionFactory
    {
        public HubConnection CreateSignalRHubConnection(UserConnectionInfo userConnectionInfo)
        {
            return new HubConnectionBuilder().WithUrl(userConnectionInfo.Url, option =>
            {
                option.AccessTokenProvider = () => Task.FromResult(userConnectionInfo.AccessToken);
            }).Build();
        }
    }
}