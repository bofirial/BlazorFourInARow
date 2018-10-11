using BlazorFourInARow.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFourInARow.BusinessLogic
{
    public interface IUserRegistrar
    {
        Task RegisterUserAsync(User user);
    }
}
