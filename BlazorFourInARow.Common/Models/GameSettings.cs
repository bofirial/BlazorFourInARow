using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameSettings
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public double TurnDelaySeconds { get; set; }

        public double GameStartDelaySeconds { get; set; }

        public int GameAbandonmentMinutes { get; set; }

        public int Teams { get; set; }

        public int PiecesInARowToWin { get; set; }
    }
}
