using System.Collections.Generic;
using System.Text;
using BlazorFourInARow.Common.Models;
using Microsoft.Azure.Documents.Client;

namespace BlazorFourInARowFunctions.Game
{
    public interface IGameBuilder
    {
        GameAction BuildNewGame(DocumentClient client);
    }
}
