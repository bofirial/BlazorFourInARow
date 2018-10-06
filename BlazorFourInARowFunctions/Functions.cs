
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class Functions
    {
        //[FunctionName("Function1")]
        //public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    return name != null
        //        ? (ActionResult)new OkObjectResult($"Hello, {name}")
        //        : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        //}

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "gameUpdates")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("game-action")]
        public static Task SendGameAction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] string message,
            [SignalR(HubName = "gameUpdates")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "game-update",
                    Arguments = new[] { message }
                });
        }
    }
}
