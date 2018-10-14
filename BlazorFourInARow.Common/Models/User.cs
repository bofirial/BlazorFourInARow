using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class User
    {
        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public string DisplayColor { get; set; }

        public Team Team { get; set; }
    }
}
