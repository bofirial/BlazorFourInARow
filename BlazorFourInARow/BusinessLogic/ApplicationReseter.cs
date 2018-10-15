using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorFourInARow.BusinessLogic
{
    public class ApplicationReseter
    {
        private readonly ILogger<UserRegistrar> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;
        private readonly LocalStorage _localStorage;

        public ApplicationReseter(ILogger<UserRegistrar> logger,
            HttpClient httpClient, IServiceBaseUrlProvider serviceBaseUrlProvider, LocalStorage localStorage)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
            _localStorage = localStorage;
        }

        public async Task ResetApplication()
        {
            _logger.LogInformation($"Reseting the Application.");

            await _localStorage.Clear();

            await _httpClient.DeleteAsync($"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/entire-database");

            await JSRuntime.Current.InvokeAsync<bool>("Refresh");
        }
    }
}
