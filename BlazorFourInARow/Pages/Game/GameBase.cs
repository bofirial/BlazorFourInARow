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
using BlazorFourInARow.Common.Validators;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BlazorFourInARow.Pages.Game
{
    public class GameBase : BlazorComponent
    {
        public GameState GameState { get; set; }

        public GameState NextGameState { get; set; }

        public UserConnectionInfo UserConnectionInfo { get; set; }

        public Team Team { get; set; }

        public DateTime? NextActionAvailableOn { get; set; }

        public bool ActionsLocked { get; set; } = false;

        public DateTime? NextGameStartOn { get; set; }

        [Inject]
        protected ICurrentGameStateProvider CurrentGameStateProvider { get; set; }

        [Inject]
        protected IUserConnectionInfoStore UserConnectionInfoStore { get; set; }

        [Inject]
        protected ISignalRConnectionFactory SignalRConnectionFactory { get; set; }

        [Inject]
        protected IGameStateManager GameStateManager { get; set; }

        [Inject]
        protected IGameJoiner GameJoiner { get; set; }

        [Inject]
        protected ILogger<GameBase> Logger { get; set; }

        protected override async Task OnInitAsync()
        {
            UserConnectionInfo = await UserConnectionInfoStore.GetUserConnectionInfoAsync();
            
            await UpdateGameState(await CurrentGameStateProvider.GetCurrentGameStateAsync());

            Logger.LogInformation($"GameState set for game: ({GameState?.GameId}): {Newtonsoft.Json.JsonConvert.SerializeObject(GameState)}");

            var connection = SignalRConnectionFactory.CreateSignalRHubConnection(UserConnectionInfo);

            connection.On<GameState>("game-state-update", this.OnGameUpdate);
            await connection.StartAsync();
        }

        protected async Task OnGameUpdate(GameState gameState)
        {
            Logger.LogInformation($"GameState updated for game: ({gameState?.GameId}): {Newtonsoft.Json.JsonConvert.SerializeObject(gameState)}");

            await UpdateGameState(gameState);

            StateHasChanged();
        }

        protected async Task UpdateGameState(GameState gameState)
        {
            if (GameState == null || GameState.GameId == gameState.GameId)
            {
                GameState = gameState;

                if (!GameStateManager.UserHasJoinedGame(gameState, UserConnectionInfo.User))
                {
                    Team = await GameJoiner.JoinGameAsync(gameState.GameId, UserConnectionInfo.User);
                }
                else
                {
                    Team = gameState.Teams.FirstOrDefault(t =>
                        t.Users.Any(u => u.UserId == UserConnectionInfo.User.UserId));
                }

                var user = Team?.Users.FirstOrDefault(u => u.UserId == UserConnectionInfo.User.UserId);

                if (user != null)
                {
                    NextActionAvailableOn = user.NextActionUnlocked;

                    if (NextActionAvailableOn > DateTime.Now)
                    {
                        ActionsLocked = true;
                    }
                }

                if (GameState.GameResult != null && NextGameState == null)
                {
                    NextGameState = new GameState()
                    {
                        GameId = GameState.GameResult.NextGameId
                    };

                    Logger.LogInformation($"Creating Next Game State: {NextGameState.GameId}");

                    NextGameStartOn = DateTime.Now.AddSeconds(gameState.GameSettings.GameStartDelaySeconds);
                }
            }
            else if (NextGameState != null && NextGameState.GameId == gameState.GameId)
            {
                Logger.LogInformation($"Logging Next Game State");

                NextGameState = gameState;

                if (!GameStateManager.UserHasJoinedGame(gameState, UserConnectionInfo.User))
                {
                    await GameJoiner.JoinGameAsync(gameState.GameId, UserConnectionInfo.User);
                }
            }
            else
            {
                Logger.LogInformation($"How did we get here?");
                Logger.LogInformation($"NextGameState Null? {NextGameState==null}");
                Logger.LogInformation($"NextGameState.GameId? {NextGameState?.GameId}");
                Logger.LogInformation($"gameState.GameId? {gameState?.GameId}");
                Logger.LogInformation($"All Together? {NextGameState != null && NextGameState.GameId == gameState.GameId}");
            }

            if (gameState.GameStart > DateTime.Now)
            {
                NextGameStartOn = gameState.GameStart;
                NextActionAvailableOn = null;
                ActionsLocked = true;
            }
        }

        public void ResetActionLock()
        {
            ActionsLocked = false;

            StateHasChanged();
        }

        public void SetActionLock()
        {
            ActionsLocked = true;

            NextActionAvailableOn = DateTime.Now.AddSeconds(GameState.GameSettings.TurnDelaySeconds + 1);

            StateHasChanged();
        }

        public void StartNextGame()
        {
            if (NextGameState != null)
            {
                GameState = NextGameState;
                NextGameState = null;
                
                Team = GameState.Teams.FirstOrDefault(t =>
                    t.Users.Any(u => u.UserId == UserConnectionInfo.User.UserId));
            }

            ResetActionLock();
        }
    }
}
