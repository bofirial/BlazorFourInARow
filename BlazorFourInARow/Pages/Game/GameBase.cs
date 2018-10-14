using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blazor.Extensions;
using BlazorFourInARow.BusinessLogic;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BlazorFourInARow.Pages.Game
{
    public class GameBase : BlazorComponent
    {
        public GameState GameState { get; set; }

        public UserConnectionInfo UserConnectionInfo { get; set; }

        [Inject]
        protected ICurrentGameStateProvider CurrentGameStateProvider { get; set; }

        [Inject]
        protected IUserConnectionInfoStore UserConnectionInfoStore { get; set; }

        [Inject]
        protected ISignalRConnectionFactory SignalRConnectionFactory { get; set; }

        [Inject]
        protected ILogger<GameBase> Logger { get; set; }

        protected override async Task OnInitAsync()
        {
            GameState = await CurrentGameStateProvider.GetCurrentGameStateAsync();

            Logger.LogInformation($"GameState set for game: ({GameState?.GameId}): {Newtonsoft.Json.JsonConvert.SerializeObject(GameState)}");

            UserConnectionInfo = await UserConnectionInfoStore.GetUserConnectionInfoAsync();

            var connection = SignalRConnectionFactory.CreateSignalRHubConnection(UserConnectionInfo);

            connection.On<GameState>("game-state-update", this.OnGameUpdate);
            await connection.StartAsync();
        }

        protected Task OnGameUpdate(GameState gameState)
        {
            Logger.LogInformation($"GameState updated for game: ({gameState?.GameId}): {Newtonsoft.Json.JsonConvert.SerializeObject(gameState)}");

            GameState = gameState;

            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
