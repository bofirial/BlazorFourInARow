using System.Collections.Generic;

namespace BlazorFourInARow.Common.Models
{
    public class Team
    {
        public string TeamId { get; set; }
        
        public string DisplayColor { get; set; }

        public List<User> Users { get; set; }
    }
}