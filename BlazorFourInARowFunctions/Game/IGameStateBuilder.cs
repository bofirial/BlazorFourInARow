using System.Collections.Generic;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARowFunctions.Game
{
    public interface IGameStateBuilder
    {
        GameState BuildGameState(List<GameAction> gameActions);
    }
}