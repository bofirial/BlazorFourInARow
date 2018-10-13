using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFourInARow.BusinessLogic;
using BlazorFourInARow.Common.Models;
using Microsoft.AspNetCore.Blazor.Components;

namespace BlazorFourInARow.Pages.Game
{
    public class GameBase : BlazorComponent
    {
        public GameState GameState { get; set; }

        [Inject]
        protected ICurrentGameStateProvider CurrentGameStateProvider { get; set; }

        protected override async Task OnInitAsync()
        {
            GameState = await CurrentGameStateProvider.GetCurrentGameStateAsync();
        }
    }
}
