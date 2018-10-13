using System.Collections.Generic;
using System.Text;
using BlazorFourInARow.Common.Models;
using Microsoft.Azure.Documents.Client;
using GameAction = BlazorFourInARowFunctions.Models.GameAction;

namespace BlazorFourInARowFunctions.Game
{
    public interface IGameBuilder
    {
        GameAction BuildNewGame(DocumentClient client);
    }
}
