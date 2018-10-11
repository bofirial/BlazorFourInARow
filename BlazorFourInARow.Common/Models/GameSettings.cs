using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameSettings
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public decimal TurnDelaySeconds { get; set; }

        public int Teams { get; set; }
    }
}
