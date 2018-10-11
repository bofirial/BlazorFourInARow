using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorFourInARowFunctions
{
    public class UserFunction
    {
        [FunctionName("user")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage req,
            [SignalRConnectionInfo(HubName = "gameUpdates")] SignalRConnectionInfo connectionInfo,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection")]
            DocumentClient client,
            ILogger log)
        {
            User user = await req.Content.ReadAsAsync<User>();

            if (null == user)
            {
                log.LogError("Missing user object in POST body.");
                return new BadRequestObjectResult("Missing user object in POST body.");
            }

            UserConnectionInfo userConnectionInfo = new UserConnectionInfo()
            {
                Url = connectionInfo.Url,
                AccessToken = connectionInfo.AccessToken,
                User = user
            };

            Uri documentCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

            using (IDocumentQuery<GameAction> documentQuery = client.CreateDocumentQuery<GameAction>(
                documentCollectionUri)
                .Where(g => g.User.UserId == user.UserId).AsDocumentQuery())
            {
                while (documentQuery.HasMoreResults)
                {
                    foreach (GameAction pg in await documentQuery.ExecuteNextAsync<GameAction>())
                    {
                        log.LogError($"A User with an id of ({user.UserId}) has already been registered.");
                        return new ConflictObjectResult($"A User with an id of ({user.UserId}) has already been registered.");
                    }
                }
            }

            //TODO: Set user.DisplayColor;

            GameAction userRegistrationGameAction = new GameAction()
            {
                User = user,
                GameActionStatus = GameActionStatuses.Valid,
                GameActionType = GameActionTypes.RegisterUser
            };

            await client.CreateDocumentAsync(documentCollectionUri, userRegistrationGameAction);

            log.LogInformation($"Created new user {user.DisplayName}. ({user.UserId})");
            
            return new OkObjectResult(userConnectionInfo);
        }
    }
}
