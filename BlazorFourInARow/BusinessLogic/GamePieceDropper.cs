﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.Extensions.Logging;

namespace BlazorFourInARow.BusinessLogic
{
    public class GamePieceDropper : IGamePieceDropper
    {
        private readonly HttpClient _httpClient;
        private readonly IUserConnectionInfoStore _userConnectionInfoStore;
        private readonly IServiceBaseUrlProvider _serviceBaseUrlProvider;
        private readonly ILogger<GamePieceDropper> _logger;

        public GamePieceDropper(HttpClient httpClient, IUserConnectionInfoStore userConnectionInfoStore,
            IServiceBaseUrlProvider serviceBaseUrlProvider, ILogger<GamePieceDropper> logger)
        {
            _httpClient = httpClient;
            _userConnectionInfoStore = userConnectionInfoStore;
            _serviceBaseUrlProvider = serviceBaseUrlProvider;
            _logger = logger;
        }

        public async Task DropGamePieceAsync(int column, string gameId, Team team)
        {
            var userConnectionInfo = await _userConnectionInfoStore.GetUserConnectionInfoAsync();

            _logger.LogInformation($"Dropping a piece in the {column} column.");

            await _httpClient.PostJsonAsync(
                $"{_serviceBaseUrlProvider.GetServiceBaseUrl()}/api/game-piece",
                new GameCell()
                {
                    Column = column,
                    User = userConnectionInfo.User,
                    GameId = gameId,
                    Team = team
                });
        }
    }
}