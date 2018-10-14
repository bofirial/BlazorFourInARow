using System;
using System.Collections.Generic;
using System.Text;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.Common.Validators
{
    public interface IGameStateManager
    {
        bool UserHasJoinedGame(GameState gameState, User user);

        GameActionStatuses ValidateGameColumnAction(GameState gameState, int column);

        GameResult CheckForGameCompletion(GameState gameState);
    }
}
