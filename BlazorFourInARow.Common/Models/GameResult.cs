using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameResult
    {
        public Team WinningTeam { get; set; }

        public string NextGameId { get; set; }
    }
}
