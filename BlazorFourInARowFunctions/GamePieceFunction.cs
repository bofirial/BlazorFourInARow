using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlazorFourInARow.Common;
using BlazorFourInARow.Common.Models;
using BlazorFourInARowFunctions.Game;
using BlazorFourInARowFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BlazorFourInARowFunctions
{
    public static class GamePieceFunction
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("game-piece")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = null)]HttpRequestMessage req,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true)]
            DocumentClient client, ILogger log)
        {
            var gamePiece = await req.Content.ReadAsAsync<GamePosition>();

            await client.CreateDocumentAsync(DocumentCollectionUri, new GameAction()
            {
                GameActionStatus = GameActionStatuses.AwaitingValidation,
                GameActionType = GameActionTypes.PlaceGamePiece,
                GameId = gamePiece.GameId,
                GamePosition = gamePiece,
                User = gamePiece.User,
                Team = gamePiece.Team
            });

            return new OkResult();
        }
    }
}
