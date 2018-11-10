using System;
using BlazorFourInARow.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace BlazorFourInARow.Common.Validators
{
    public class GameStateManager : IGameStateManager
    {
        public bool UserHasJoinedGame(GameState gameState, User user)
        {
            var usersInGame = gameState.Teams.SelectMany(t => t.Users);

            return usersInGame.Any(u => u.UserId == user.UserId);
        }

        public GameActionStatuses ValidateGameColumnAction(GameState gameState, int column)
        {

            if (GameStateColumnIsFull(gameState, column))
            {
                return GameActionStatuses.InvalidColumnFull;
            }

            if (null != gameState.GameResult)
            {
                return GameActionStatuses.InvalidGameHasEnded;
            }

            return GameActionStatuses.Valid;
        }

        private static bool GameStateColumnIsFull(GameState gameState, int column)
        {
            foreach (var row in gameState.GameCells)
            {
                var gameCell = row[column];

                if (null == gameCell.Team)
                {
                    return false;
                }
            }

            return true;
        }

        public GameResult CheckForGameCompletion(GameState gameState)
        {
            var gameBoardIsFull = true;

            for (var row = 0; row < gameState.GameSettings.Rows; row++)
            {
                for (var column = 0; column < gameState.GameSettings.Columns; column++)
                {
                    var gameCell = gameState.GameCells[row][column];

                    if (null == gameCell.Team)
                    {
                        gameBoardIsFull = false;

                        continue;
                    }

                    if (GameCellIsStartOfVictoryRow(gameCell, gameState))
                    {
                        return new GameResult() { WinningTeam = gameCell.Team };
                    }
                }
            }

            if (gameBoardIsFull)
            {
                return new GameResult();
            }

            return null;
        }

        public bool GameCellIsStartOfVictoryRow(GameCell gameCell, GameState gameState)
        {
            if (gameCell.Team == null)
            {
                return false;
            }

            var victoryConditions = new List<VictoryCondition>()
            {
                new VictoryCondition() {IsMatch = true, RowTransform = 1, ColumnTransform = 0}, //Horizontal
                new VictoryCondition() {IsMatch = true, RowTransform = 0, ColumnTransform = 1}, //Vertical
                new VictoryCondition() {IsMatch = true, RowTransform = -1, ColumnTransform = 1}, //Diagonal Up Left
                new VictoryCondition() {IsMatch = true, RowTransform = 1, ColumnTransform = 1}, //Diagonal Up Right
            };

            for (var i = 1; i < gameState.GameSettings.PiecesInARowToWin; i++)
            {
                foreach (var victoryCondition in victoryConditions.Where(vc => vc.IsMatch))
                {
                    if (null == gameCell.Row)
                    {
                        throw new ApplicationException("Invalid Game Position in Game State - Missing Row"); 
                    }

                    var targetRow = gameCell.Row.Value + i * victoryCondition.RowTransform;
                    var targetColumn = gameCell.Column + i * victoryCondition.ColumnTransform;

                    if (!IsValidGamePosition(gameState, targetRow, targetColumn))
                    {
                        victoryCondition.IsMatch = false;
                        continue;
                    }

                    var targetCell = gameState.GameCells[targetRow][targetColumn];

                    victoryCondition.IsMatch = targetCell?.Team?.TeamId == gameCell.Team.TeamId;
                }
            }

            return victoryConditions.Any(v => v.IsMatch);
        }

        private static bool IsValidGamePosition(GameState gameState, int targetRow, int targetColumn)
        {
            return targetRow >= 0 && targetRow < gameState.GameSettings.Rows && targetColumn >= 0 &&
                   targetColumn < gameState.GameSettings.Columns;
        }
    }
}
