using System;
using BlazorFourInARow.Common;
using BlazorFourInARow.Common.Models;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using User = BlazorFourInARow.Common.Models.User;

namespace BlazorFourInARowFunctions.Models
{
    public class GameAction
    {
        public string GameId { get; set; }

        public User User { get; set; }

        public Team Team { get; set; }

        public GamePosition GamePosition { get; set; }

        public GameActionStatuses GameActionStatus { get; set; }

        public GameActionTypes GameActionType { get; set; }

        public GameSettings GameSettings { get; set; }

        public DateTime CreatedOn { get; } = DateTime.Now;

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
