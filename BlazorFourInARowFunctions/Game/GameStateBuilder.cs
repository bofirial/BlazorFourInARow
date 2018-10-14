using System;
using System.Collections.Generic;
using System.Linq;
using BlazorFourInARow.Common;
using BlazorFourInARow.Common.Models;
using BlazorFourInARowFunctions.Models;
using GameAction = BlazorFourInARowFunctions.Models.GameAction;
using GameActionTypes = BlazorFourInARowFunctions.Models.GameActionTypes;

namespace BlazorFourInARowFunctions.Game
{
    public class GameStateBuilder : IGameStateBuilder
    {
        public GameState BuildGameState(List<GameAction> gameActions)
        {
            var createGameAction = gameActions.FirstOrDefault(g => g.GameActionType == GameActionTypes.CreateGame);

            if (null == createGameAction)
            {
                throw new ApplicationException("Missing CreateGameAction");
            }

            var gameState = CreateGameState(createGameAction);

            foreach (var gameAction in gameActions)
            {
                if (gameAction.GameActionStatus != GameActionStatuses.Valid)
                {
                    continue;
                }

                switch (gameAction.GameActionType)
                {
                    case GameActionTypes.JoinGame:
                        gameState.Teams.First(t => t.TeamId == gameAction.Team.TeamId).Users.Add(gameAction.User);
                        break;
                    case GameActionTypes.LeaveGame:
                        break;
                    case GameActionTypes.PlaceGamePiece:
                        var gameCell = gameState.GameCells[gameAction.GamePosition.Row.Value][gameAction.GamePosition.Column];

                        gameCell.Team = gameAction.Team;
                        gameCell.User = gameAction.User;
                        break;
                    case GameActionTypes.CompleteGame:
                        break;
                    case GameActionTypes.AbandonGame:
                        break;
                    case GameActionTypes.RegisterUser:
                    case GameActionTypes.CreateGame:
                    default:
                        break;
                }
            }

            gameState.DebugMessage = $"{gameActions.Count} Game Actions in this game ({createGameAction.GameId}).";

            return gameState;
        }

        private static GameState CreateGameState(GameAction createGameAction)
        {
            var gameState = new GameState
            {
                GameId = createGameAction.GameId,
                GameCells = new List<List<GameCell>>(),
                Teams = new List<Team>()
                {
                    new Team()
                    {
                        TeamId = "Red",
                        DisplayColor = "Red",
                        Users = new List<User>()
                    },
                    new Team()
                    {
                        TeamId = "Black",
                        DisplayColor = "Black",
                        Users = new List<User>()
                    }
                }
            };


            for (int i = 0; i < createGameAction.GameSettings.Rows; i++)
            {
                gameState.GameCells.Add(new List<GameCell>());

                for (int j = 0; j < createGameAction.GameSettings.Columns; j++)
                {
                    gameState.GameCells[i].Add(new GameCell()
                    {
                        GamePosition = new GamePosition()
                        {
                            Row = i,
                            Column = j
                        }
                    });
                }
            }

            return gameState;
        }
    }
}
