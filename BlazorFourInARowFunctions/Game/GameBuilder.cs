using System;
using BlazorFourInARow.Common.Models;
using Microsoft.Azure.Documents.Client;
using GameAction = BlazorFourInARowFunctions.Models.GameAction;
using GameActionStatuses = BlazorFourInARow.Common.GameActionStatuses;
using GameActionTypes = BlazorFourInARowFunctions.Models.GameActionTypes;

namespace BlazorFourInARowFunctions.Game
{
    public class GameBuilder : IGameBuilder
    {
        public GameAction BuildNewGame(DocumentClient client)
        {
            //TODO: Allow Admins to submit a GameAction to alter Game Settings

            return new GameAction()
            {
                GameActionType = GameActionTypes.CreateGame,
                GameId = Guid.NewGuid().ToString(),
                GameActionStatus = GameActionStatuses.Valid,
                GameSettings = new GameSettings()
                {
                    Columns = 7,
                    Rows = 6,
                    Teams = 2,
                    TurnDelaySeconds = 3
                }
            };
        }
    }
}