using System.Collections.Generic;
using System.Linq;
using Blazor.Extensions;
using BlazorFourInARow.Common.Models;

namespace BlazorFourInARow.BusinessLogic
{
    public interface ISignalRConnectionFactory
    {
        HubConnection CreateSignalRHubConnection(UserConnectionInfo userConnectionInfo);
    }
}
