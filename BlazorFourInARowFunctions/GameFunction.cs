
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;
using BlazorFourInARowFunctions.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class GameFunction
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("game")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = null)]HttpRequest req,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection")]
            DocumentClient client, ILogger log)
        {
            Uri documentCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

            var gameStateBuilder = new GameStateBuilder();

            var currentGame = client.CreateDocumentQuery<GameAction>(documentCollectionUri)
                .Where(g => g.GameActionType == GameActionTypes.CreateGame)
                .OrderBy(g => g.CreatedOn)
                .AsEnumerable()
                .FirstOrDefault();

            if (null != currentGame)
            {
                var gameActions = client.CreateDocumentQuery<GameAction>(documentCollectionUri)
                    .Where(g => g.GameId == currentGame.GameId)
                    .AsEnumerable().ToList();

                if (!gameActions.Any(g =>
                    g.GameActionType == GameActionTypes.CompleteGame ||
                    g.GameActionType == GameActionTypes.AbandonGame))
                {

                    return new OkObjectResult(gameStateBuilder.BuildGameState(gameActions));
                }
            }

            var gameAction = await CreateNewGameAction(client);

            return new OkObjectResult(gameStateBuilder.BuildGameState(new List<GameAction>() { gameAction }));
        }

        private static async Task<GameAction> CreateNewGameAction(DocumentClient client)
        {
            var gameBuilder = new GameBuilder();

            var newGameAction = gameBuilder.BuildNewGame(client);

            await client.CreateDocumentAsync(DocumentCollectionUri, newGameAction);

            return newGameAction;
        }
    }
}
