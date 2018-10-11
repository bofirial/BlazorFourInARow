using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public enum GameActionTypes
    {
        RegisterUser = 1,
        CreateGame = 2,
        JoinGame = 3,
        LeaveGame = 4,
        PlaceGamePiece = 5,
        CompleteGame = 6,
        AbandonGame = 7
    }
}
