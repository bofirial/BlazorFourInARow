using System.Collections.Generic;
using BlazorFourInARow.Common.Models;
using GameAction = BlazorFourInARowFunctions.Models.GameAction;

namespace BlazorFourInARowFunctions.Game
{
    public interface IGameStateBuilder
    {
        GameState BuildGameState(List<GameAction> gameActions);
    }
}