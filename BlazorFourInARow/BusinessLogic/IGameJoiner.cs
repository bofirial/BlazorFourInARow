using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.BusinessLogic
{
    public interface IGameJoiner
    {
        Task<Team> JoinGameAsync(string gameId, User user);
    }
}
