using System.Collections.Generic;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARowFunctions.Game
{
    public class GameStateBuilder : IGameStateBuilder
    {
        public GameState BuildGameState(List<GameAction> gameActions)
        {
            return new GameState();
        }
    }
}
