using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class GameAction
    {
        public string GameId { get; set; }

        public User User { get; set; }

        public GamePosition GamePosition { get; set; }

        public GameActionStatuses GameActionStatus { get; set; }

        public GameActionTypes GameActionType { get; set; }
    }
}
