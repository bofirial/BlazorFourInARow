using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlazorFourInARowFunctions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class PlaceGamePieceGameActionValidator
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("place-game-piece-game-action-validator")]
        public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "blazor-four-in-a-row",
            collectionName: "game-actions",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection")]
            DocumentClient client,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);

                foreach (var document in input)
                {
                    GameAction gameAction = (dynamic)document;

                    if (gameAction.GameActionStatus == GameActionStatuses.AwaitingValidation)
                    {
                        log.LogInformation($"Validating Game Action {gameAction.Id}.");

                        //TODO: Validate Game Action Here

                        gameAction.GameActionStatus = GameActionStatuses.Valid;

                        log.LogInformation($"Game Action {gameAction.Id} is valid.");

                        await client.UpsertDocumentAsync(DocumentCollectionUri, gameAction);

                        //TODO: Check for Game Victory

                        continue;
                    }

                    log.LogInformation($"Game Action {gameAction.Id} does not require validation.  GameActionType: {gameAction.GameActionType}");
                }

                //TODO: Send SignalR GameState to Clients
            }
        }
    }
}
