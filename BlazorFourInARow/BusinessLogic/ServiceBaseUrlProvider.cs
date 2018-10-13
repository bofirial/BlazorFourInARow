using Microsoft.AspNetCore.Blazor.Services;

namespace BlazorFourInARow.BusinessLogic
{
    public class ServiceBaseUrlProvider : IServiceBaseUrlProvider
    {
        private readonly IUriHelper _uriHelper;

        public ServiceBaseUrlProvider(IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
        }

        public string GetServiceBaseUrl()
        {
            if (_uriHelper.GetBaseUri().Contains("localhost"))
            {
                return "http://localhost:7071";
            }

            return "https://blazorfourinarowfunctionapp.azurewebsites.net";
        }
    }
}