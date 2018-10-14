using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.Extensions.Logging;

namespace BlazorFourInARow.BusinessLogic
{
    public class GameJoiner : IGameJoiner
    {
        private readonly ILogger<GameJoiner> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;

        public GameJoiner(ILogger<GameJoiner> logger,
            HttpClient httpClient, IServiceBaseUrlProvider serviceBaseUrlProvider)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
        }

        public async Task<Team> JoinGameAsync(string gameId, User user)
        {
            var team = await _httpClient.PostJsonAsync<Team>(
                $"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/game-player", (GameId: gameId, User: user));

            _logger.LogInformation($"Joined the game ({gameId}) on the {team.TeamId} team.");

            return team;
        }
    }
}