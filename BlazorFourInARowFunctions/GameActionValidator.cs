using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorFourInARowFunctions.Game;
using BlazorFourInARowFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class GameActionValidator
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("game-action-validator")]
        public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "blazor-four-in-a-row",
            collectionName: "game-actions",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true,
                FeedPollDelay = 200)]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true)]
            DocumentClient client,
            [SignalR(HubName = "gameUpdates")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("GameActions modified " + input.Count);

                var gameStateBuilder = new GameStateBuilder();
                var gameActionsProvider = new GameActionsProvider();

                var updatedGameIds = new List<string>();

                foreach (var document in input)
                {
                    GameAction gameAction = (dynamic)document;

                    if (!updatedGameIds.Contains(gameAction.GameId))
                    {
                        updatedGameIds.Add(gameAction.GameId);
                    }

                    if (gameAction.GameActionStatus == GameActionStatuses.AwaitingValidation)
                    {
                        log.LogInformation($"Validating Game Action {gameAction.Id}.");

                        var gameActions = gameActionsProvider.GetGameActions(client, gameAction.GameId);
                        var gameState = gameStateBuilder.BuildGameState(gameActions);

                        //TODO: Validate Game Action Here
                        gameAction.GameActionStatus = GameActionStatuses.Valid;


                        foreach (var row in gameState.GameCells)
                        {
                            var gameCell = row[gameAction.GamePosition.Column];

                            if (null == gameCell.Team)
                            {
                                gameAction.GamePosition.Row = gameCell.GamePosition.Row;
                                break;
                            }
                        }
                        
                        log.LogInformation($"Game Action {gameAction.Id} is valid.");

                        await client.UpsertDocumentAsync(DocumentCollectionUri, gameAction);

                        //TODO: Check for Game Victory

                        continue;
                    }

                    log.LogInformation($"Game Action {gameAction.Id} does not require validation.  GameActionType: {gameAction.GameActionType}");
                }

                foreach (var gameId in updatedGameIds)
                {
                    var gameActions = gameActionsProvider.GetGameActions(client, gameId);

                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "game-state-update",
                            Arguments = new object[] { gameStateBuilder.BuildGameState(gameActions) }
                        }); 
                }
            }
        }
    }
}
