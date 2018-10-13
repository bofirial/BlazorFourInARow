namespace BlazorFourInARow.Common.Models
{
    public class GameCell
    {
        public GamePosition GamePosition { get; set; }

        public User User { get; set; }

        public Team Team { get; set; }
    }
}