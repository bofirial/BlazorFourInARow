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
using GameAction = BlazorFourInARowFunctions.Models.GameAction;
using GameActionStatuses = BlazorFourInARow.Common.GameActionStatuses;
using GameActionTypes = BlazorFourInARowFunctions.Models.GameActionTypes;

namespace BlazorFourInARowFunctions
{
    public class UserFunction
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        [FunctionName("user")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage req,
            [SignalRConnectionInfo(HubName = "gameUpdates")] SignalRConnectionInfo connectionInfo,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true)]
            DocumentClient client,
            ILogger log)
        {
            User user = await req.Content.ReadAsAsync<User>();

            if (null == user)
            {
                log.LogError("Missing user object in POST body.");
                return new BadRequestObjectResult("Missing user object in POST body.");
            }

            if (!UserRegistrationGameActionAlreadyExists(client, user))
            {
                await StoreRegisterUserGameAction(client, user);

                log.LogInformation($"Created new user {user.DisplayName}. ({user.UserId})");
            }

            //TODO: Set user.DisplayColor;

            return new OkObjectResult(new UserConnectionInfo()
            {
                Url = GetSignalRUrl(connectionInfo),
                AccessToken = connectionInfo.AccessToken,
                User = user
            });
        }

        private static string GetSignalRUrl(SignalRConnectionInfo connectionInfo)
        {
            //Drop the port from the SignalRConnectionInfo to use 443 instead.
            return new Uri(connectionInfo.Url).GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port,
                                UriFormat.UriEscaped);
        }

        private static bool UserRegistrationGameActionAlreadyExists(DocumentClient client, User user)
        {
            var userRegistrationGameAction = client
                .CreateDocumentQuery<GameAction>(DocumentCollectionUri)
                .Where(g => g.User.UserId == user.UserId && g.GameActionType == GameActionTypes.RegisterUser)
                .AsEnumerable().FirstOrDefault();

            return null != userRegistrationGameAction;
        }

        private static async Task StoreRegisterUserGameAction(DocumentClient client, User user)
        {
            await client.CreateDocumentAsync(DocumentCollectionUri, new GameAction()
            {
                User = user,
                GameActionStatus = GameActionStatuses.Valid,
                GameActionType = GameActionTypes.RegisterUser
            });
        }
    }
}
