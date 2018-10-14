using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.Common.Validators
{
    public class GameStateValidator : IGameStateValidator
    {
        public bool UserHasJoinedGame(GameState gameState, User user)
        {
            return gameState.Teams.SelectMany(t => t.Users).Any(u => u.UserId == user.UserId);
        }
    }
}
