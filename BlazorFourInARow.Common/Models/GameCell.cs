namespace BlazorFourInARow.Common.Models
{
    public class GameCell
    {
        public string GameId { get; set; }

        public int? Row { get; set; }

        public int Column { get; set; }

        public User User { get; set; }

        public Team Team { get; set; }
    }
}