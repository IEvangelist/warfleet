using IEvangelist.Blazing.WarFleet.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class Lobby : IAsyncDisposable
    {
        HubConnection? _serverConnection;
        IDictionary<string, Game>? _joinableGames = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient Http { get; set; } = null!;

        public bool HasValidName => PlayerName is { Length: >= 3 };
        public string PlayerName { get; set; } = null!;
        
        public HubConnectionState ConnectionState =>
            _serverConnection?.State ?? HubConnectionState.Disconnected;

        protected override async Task OnInitializedAsync()
        {
            _joinableGames =
                await Http.GetFromJsonAsync<IDictionary<string, Game>>("api/games/joinable");

            _serverConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            await _serverConnection.OnJoinableGamesUpdated(OnJoinableGamesUpdatedAsync)
                .StartAsync();
        }

        async Task OnJoinableGamesUpdatedAsync(IDictionary<string, Game> games) =>
            await InvokeAsync(() =>
            {
                _joinableGames = games;

                StateHasChanged();
            });

        public void StartGame(BoardSize size) =>
            TryNavigateTo(
                $"game/{Guid.NewGuid()}?newGame=true&playerName={PlayerName}&boardSize={size}");

        public void TryJoinGame(string gameId) =>
            TryNavigateTo(
                $"game/{gameId}?playerName={PlayerName}");

        void TryNavigateTo(string route)
        {
            if (PlayerName is not { Length: >= 3 })
            {
                return;
            }

            NavigationManager.NavigateTo(route);
        }

        public async ValueTask DisposeAsync()
            => await (_serverConnection?.DisposeAsync() ?? new ValueTask());
    }
}
