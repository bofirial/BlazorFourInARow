using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.Common.Validators
{
    public class GameStateManager : IGameStateManager
    {
        public bool UserHasJoinedGame(GameState gameState, User user)
        {
            return gameState.Teams.SelectMany(t => t.Users).Any(u => u.UserId == user.UserId);
        }

        public GameActionStatuses ValidateGameColumnAction(GameState gameState, int column)
        {
            var columnFull = true;

            foreach (var row in gameState.GameCells)
            {
                var gameCell = row[column];

                if (null == gameCell.Team)
                {
                    columnFull = false;
                    break;
                }
            }

            //TODO: Validate Game is Over
            //TODO: Validate GameAction Is Too Soon

            if (columnFull)
            {
                return GameActionStatuses.InvalidColumnFull;
            }

            return GameActionStatuses.Valid;
        }
    }
}
