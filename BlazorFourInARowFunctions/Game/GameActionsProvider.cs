using System;
using System.Collections.Generic;
using System.Linq;
using BlazorFourInARowFunctions.Models;
using Microsoft.Azure.Documents.Client;

namespace BlazorFourInARowFunctions.Game
{
    public class GameActionsProvider : IGameActionsProvider
    {
        private static readonly Uri DocumentCollectionUri =
            UriFactory.CreateDocumentCollectionUri(databaseId: "blazor-four-in-a-row", collectionId: "game-actions");

        public List<GameAction> GetGameActions(DocumentClient client, string gameId)
        {
            return client.CreateDocumentQuery<GameAction>(DocumentCollectionUri)
                .Where(g => g.GameId == gameId)
                .AsEnumerable().ToList();
        }
    }
}