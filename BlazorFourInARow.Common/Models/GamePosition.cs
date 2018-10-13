using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GamePosition
    {
        public string GameId { get; set; }

        public int? Row { get; set; }

        public int Column { get; set; }

        public User User { get; set; }
    }
}
