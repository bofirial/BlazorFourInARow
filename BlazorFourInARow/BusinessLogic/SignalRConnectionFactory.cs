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
                option.AccessTokenProvider = () =>
                {
                    return Task.FromResult(userConnectionInfo.AccessToken);
                    //var jwtTokenHandler = new JwtSecurityTokenHandler();
                    //var accessKey = userConnectionInfo.AccessToken;

                    //var token = jwtTokenHandler.CreateJwtSecurityToken(
                    //    issuer: null,
                    //    audience: userConnectionInfo.Url,
                    //    subject: new ClaimsIdentity(new[]
                    //    {
                    //        new Claim(ClaimTypes.NameIdentifier, userConnectionInfo.User.DisplayName)
                    //    }),
                    //    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                    //    signingCredentials: new SigningCredentials(
                    //        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey)),
                    //        SecurityAlgorithms.HmacSha256));

                    //return Task.FromResult(jwtTokenHandler.WriteToken(token));
                };
            }).Build();
        }
    }
}