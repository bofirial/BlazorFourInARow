using BlazorFourInARow.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFourInARow.BusinessLogic
{
    public interface IUserConnectionInfoStore
    {
        Task<UserConnectionInfo> GetUserConnectionInfo();

        Task SetUserConnectionInfo(UserConnectionInfo userConnectionInfo);

        event Action<UserConnectionInfo> UserConnectionInfoChanged;
    }
}
