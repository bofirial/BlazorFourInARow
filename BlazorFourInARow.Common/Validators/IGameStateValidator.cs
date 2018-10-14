using System;
using System.Collections.Generic;
using System.Text;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.Common.Validators
{
    public interface IGameStateValidator
    {
        bool UserHasJoinedGame(GameState gameState, User user);
    }
}
