using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorFourInARowFunctions
{
    public class UserFunction
    {
        [FunctionName("user")]
        public static async Task<UserConnectionInfo> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage req,
            [SignalRConnectionInfo(HubName = "gameUpdates")] SignalRConnectionInfo connectionInfo,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<GameAction> gameActionDocuments)
        {
            var user = await req.Content.ReadAsAsync<User>();

            //TODO: Set user.DisplayColor;

            var userRegistrationGameAction = new GameAction()
            {
                User = user,
                GameActionStatus = GameActionStatuses.Valid,
                GameActionType = GameActionTypes.RegisterUser
            };

            await gameActionDocuments.AddAsync(userRegistrationGameAction);

            var userConnectionInfo = new UserConnectionInfo()
            {
                Url = connectionInfo.Url,
                AccessToken = connectionInfo.AccessToken,
                User = user
            };

            return userConnectionInfo;
        }
    }
}
