using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorFourInARow.Common.Models
{
    public class UserConnectionInfo
    {
        public string Url { get; set; }

        public string AccessToken { get; set; }

        public User User { get; set; }
    }
}
