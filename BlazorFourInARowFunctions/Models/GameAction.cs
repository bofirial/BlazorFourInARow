﻿using System;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARowFunctions.Models
{
    public class GameAction
    {
        public string GameId { get; set; }

        public User User { get; set; }

        public GamePosition GamePosition { get; set; }

        public GameActionStatuses GameActionStatus { get; set; }

        public GameActionTypes GameActionType { get; set; }

        public GameSettings GameSettings { get; set; }

        public DateTime CreatedOn { get; } = DateTime.Now;
    }
}