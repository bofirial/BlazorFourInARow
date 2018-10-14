using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorFourInARow.Common;
using BlazorFourInARow.Common.Models;
using BlazorFourInARow.Common.Validators;
using BlazorFourInARowFunctions.Game;
using BlazorFourInARowFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class GamePlayerFunction
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("game-player")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = null)] HttpRequestMessage req,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true)]
            DocumentClient client,
            ILogger log)
        {

            var gamePlayer = await req.Content.ReadAsAsync<(string GameId, User User)> ();
            
            var gameStateBuilder = new GameStateBuilder();
            var gameActionsProvider = new GameActionsProvider();

            var gameActions = gameActionsProvider.GetGameActions(client, gamePlayer.GameId);

            var gameState = gameStateBuilder.BuildGameState(gameActions);

            var gameStateManager = new GameStateManager();

            if (gameStateManager.UserHasJoinedGame(gameState, gamePlayer.User))
            {
                return new ConflictResult();
            }

            var teamToJoin = DetermineWhichTeamToJoin(gameState);

            teamToJoin.Users.Add(gamePlayer.User);

            await client.CreateDocumentAsync(DocumentCollectionUri, new GameAction()
            {
                GameActionStatus = GameActionStatuses.Valid,
                GameActionType = GameActionTypes.JoinGame,
                GameId = gamePlayer.GameId,
                User = gamePlayer.User,
                Team = teamToJoin
            });

            return new OkObjectResult(teamToJoin);
        }

        private static Team DetermineWhichTeamToJoin(GameState gameState)
        {
            var teams = gameState.Teams.Select(t => (Team: t, UserCount: t.Users.Count)).OrderBy(t => t.UserCount).ToList();

            var availableTeams = teams.Where(t => t.UserCount == teams[0].UserCount).ToList();

            var rand = new Random();

            return availableTeams[rand.Next(availableTeams.Count)].Team;
        }
    }
}
