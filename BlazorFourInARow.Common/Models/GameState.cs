using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameState
    {
        public string GameId { get; set; }

        public DateTime GameStart { get; set; }

        public List<List<GameCell>> GameCells { get; set; }

        public List<Team> Teams { get; set; }

        public GameResult GameResult { get; set; }

        public GameSettings GameSettings { get; set; }
    }
}
