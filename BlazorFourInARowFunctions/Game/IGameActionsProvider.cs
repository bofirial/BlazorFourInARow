using System.Collections.Generic;
using System.Text;
using BlazorFourInARowFunctions.Models;
using Microsoft.Azure.Documents.Client;

namespace BlazorFourInARowFunctions.Game
{
    public interface IGameActionsProvider
    {
        List<GameAction> GetGameActions(DocumentClient client, string gameId);
    }
}
