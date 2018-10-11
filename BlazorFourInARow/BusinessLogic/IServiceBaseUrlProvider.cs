using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFourInARow.BusinessLogic
{
    public interface IServiceBaseUrlProvider
    {
        string GetServiceBaseUrl();
    }
}
