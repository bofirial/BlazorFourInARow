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

            if (columnFull)
            {
                return GameActionStatuses.InvalidColumnFull;
            }

            //TODO: Validate Game is Over
            //TODO: Validate GameAction Is Too Soon

            return GameActionStatuses.Valid;
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
            var victoryConditions = new List<(bool IsMatch, int RowTransform, int ColumnTransform)>()
            {
                (IsMatch: true, RowTransform: 1, ColumnTransform: 0), //Horizontal
                (IsMatch: true, RowTransform: 0, ColumnTransform: 1), //Vertical
                (IsMatch: true, RowTransform: -1, ColumnTransform: 1), //Diagonal Up Left
                (IsMatch: true, RowTransform: 1, ColumnTransform: 1), //Diagonal Up Right
            };

            for (var i = 0; i < gameState.GameSettings.PiecesInARowToWin; i++)
            {
                for (var j = 0; j < victoryConditions.Count; j++)
                {
                    var victoryCondition = victoryConditions[j];

                    if (victoryCondition.IsMatch)
                    {
                        if (null == gameCell.GamePosition.Row)
                        {
                            throw new ApplicationException("Invalid Game Position in Game State - Missing Row"); 
                        }

                        var targetRow = gameCell.GamePosition.Row.Value + i * victoryCondition.RowTransform;
                        var targetColumn = gameCell.GamePosition.Column + i * victoryCondition.ColumnTransform;

                        if (targetRow < 0 || targetRow >= gameState.GameSettings.Rows || targetColumn < 0 ||
                            targetColumn >= gameState.GameSettings.Columns)
                        {
                            victoryCondition.IsMatch = false;
                            continue;
                        }

                        var targetCell = gameState.GameCells[targetRow][targetColumn];

                        victoryCondition.IsMatch = targetCell.Team.TeamId == gameCell.Team.TeamId;
                    }
                }
            }

            return victoryConditions.Any(v => v.IsMatch);
        }
    }
}
