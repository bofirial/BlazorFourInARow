using System;
using System.Collections.Generic;
using System.Linq;
using BlazorFourInARow.Common.Models;
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

            var gameState = new GameState
            {
                GameCells = new List<List<GameCell>>()
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
                        //TODO: Populate Team and Other Properties
                    });
                }
            }

            return gameState;
        }
    }
}
