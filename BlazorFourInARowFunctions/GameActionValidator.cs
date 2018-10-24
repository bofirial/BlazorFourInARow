using BlazorFourInARow.Common;
using BlazorFourInARow.Common.Validators;
using BlazorFourInARowFunctions.Game;
using BlazorFourInARowFunctions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;

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
                var gameStateManager = new GameStateManager();

                var updatedGameIds = new List<string>();

                foreach (var document in input)
                {
                    GameAction gameAction = (dynamic)document;

                    if (!updatedGameIds.Contains(gameAction.GameId))
                    {
                        updatedGameIds.Add(gameAction.GameId);
                    }

                    if (gameAction.GameActionStatus != GameActionStatuses.AwaitingValidation)
                    {
                        log.LogInformation($"Game Action {gameAction.Id} does not require validation.  GameActionType: {gameAction.GameActionType}");

                        continue;
                    }

                    log.LogInformation($"Validating Game Action {gameAction.Id}.");

                    var gameActions = gameActionsProvider.GetGameActions(client, gameAction.GameId);
                    var gameState = gameStateBuilder.BuildGameState(gameActions);

                    gameAction.GameActionStatus = gameStateManager.ValidateGameColumnAction(gameState, gameAction.GamePosition.Column);

                    ValidateGameStartLock(gameState, gameAction);
                    ValidateNextActionLock(gameState, gameAction);

                    log.LogInformation(
                        $"Game Action {gameAction.Id}'s status is {Enum.GetName(typeof(GameActionStatuses), gameAction.GameActionStatus)}.");

                    if (gameAction.GameActionStatus == GameActionStatuses.Valid)
                    {
                        foreach (var row in gameState.GameCells)
                        {
                            var gameCell = row[gameAction.GamePosition.Column];

                            if (null == gameCell.Team)
                            {
                                gameAction.GamePosition.Row = gameCell.GamePosition.Row;

                                gameCell.Team = gameAction.Team;
                                gameCell.User = gameAction.User;
                                break;
                            }
                        } 
                    }

                    await client.UpsertDocumentAsync(DocumentCollectionUri, gameAction);

                    if (null == gameState.GameResult)
                    {
                        var gameResult = gameStateManager.CheckForGameCompletion(gameState);

                        if (null != gameResult)
                        {
                            var gameBuilder = new GameBuilder();

                            var newGameAction = gameBuilder.BuildNewGame(client);

                            await client.CreateDocumentAsync(DocumentCollectionUri, new GameAction()
                            {
                                GameId = gameAction.GameId,
                                GameActionStatus = GameActionStatuses.Valid,
                                GameActionType = GameActionTypes.CompleteGame,
                                Team = gameResult.WinningTeam,
                                NextGameId = newGameAction.GameId
                            });

                            await client.CreateDocumentAsync(DocumentCollectionUri, newGameAction);
                        }
                    }
                }

                foreach (var gameId in updatedGameIds)
                {
                    if (string.IsNullOrEmpty(gameId))
                    {
                        continue;
                    }

                    var gameActions = gameActionsProvider.GetGameActions(client, gameId);

                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "game-state-update",
                            Arguments = new object[] { gameStateBuilder.BuildGameState(gameActions) }
                        });
                }

                //TODO: Task.WhenAll these awaits?
            }
        }
        private static void ValidateGameStartLock(GameState gameState, GameAction gameAction)
        {
            if (gameState.GameStart > DateTime.Now.AddSeconds(-1))
            {
                gameAction.GameActionStatus = GameActionStatuses.InvalidTooSoon;
            }
        }

        private static void ValidateNextActionLock(GameState gameState, GameAction gameAction)
        {
            var nextActionUnlocked = gameState.Teams.FirstOrDefault(t => t.TeamId == gameAction.Team.TeamId)
                ?.Users?.FirstOrDefault(u => u.UserId == gameAction.User.UserId)?.NextActionUnlocked;

            if (nextActionUnlocked > DateTime.Now.AddSeconds(-1))
            {
                gameAction.GameActionStatus = GameActionStatuses.InvalidTooSoon;
            }
        }
    }
}
