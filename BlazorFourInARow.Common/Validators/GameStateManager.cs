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

            if (null != gameState.GameResult)
            {
                return GameActionStatuses.InvalidGameHasEnded;
            }

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

                    if (gameCell.Team != null && GameCellIsStartOfVictoryRow(gameCell, gameState))
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
                //for (var j = 0; j < victoryConditions.Count; j++)
                foreach (var victoryCondition in victoryConditions)
                {
                    //var victoryCondition = victoryConditions[j];

                    if (victoryCondition.IsMatch)
                    {
                        if (null == gameCell.Row)
                        {
                            throw new ApplicationException("Invalid Game Position in Game State - Missing Row"); 
                        }

                        var targetRow = gameCell.Row.Value + i * victoryCondition.RowTransform;
                        var targetColumn = gameCell.Column + i * victoryCondition.ColumnTransform;

                        if (targetRow < 0 || targetRow >= gameState.GameSettings.Rows || targetColumn < 0 ||
                            targetColumn >= gameState.GameSettings.Columns)
                        {
                            victoryCondition.IsMatch = false;
                            continue;
                        }

                        var targetCell = gameState.GameCells[targetRow][targetColumn];

                        victoryCondition.IsMatch = victoryCondition.IsMatch && targetCell?.Team?.TeamId == gameCell.Team.TeamId;
                    }
                }
            }

            return victoryConditions.Any(v => v.IsMatch);
        }
    }

    public class VictoryCondition
    {
        public bool IsMatch { get; set; }

        public short RowTransform { get; set; }

        public short ColumnTransform { get; set; }
    }
}
