using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameResult
    {
        public Team WinningTeam { get; set; }

        //TODO: Update App to use this property
        public GameState NextGame { get; set; }
    }
}
