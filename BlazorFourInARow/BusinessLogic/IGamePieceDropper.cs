using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFourInARow.BusinessLogic
{
    public interface IGamePieceDropper
    {
        Task DropGamePieceAsync(int column, string gameId);
    }
}
