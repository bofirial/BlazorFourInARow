using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlazorFourInARowFunctions
{
    public static class EntireDatabaseFunction
    {
        private static readonly Uri StoredProcedureUri =
            UriFactory.CreateStoredProcedureUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions",
                storedProcedureId: "BulkDelete");

        [FunctionName("entire-database")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
            ILogger log,
            [CosmosDB(
                databaseName: "blazor-four-in-a-row",
                collectionName: "game-actions",
                ConnectionStringSetting = "CosmosDBConnection",
                CreateIfNotExists = true)]
            DocumentClient client)
        {
            await client.ExecuteStoredProcedureAsync<object>(StoredProcedureUri);

            return new NoContentResult();
        }
    }
}
