using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class Lobby : IAsyncDisposable
    {
        HubConnection? _hubConnection;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public bool HasValidName => PlayerName is { Length: >= 3 };
        public string PlayerName { get; set; } = null!;
        public IDictionary<string, Game>? JoinableGames { get; set; } = null!;
        public HubConnectionState ConnectionState =>
            _hubConnection?.State ?? HubConnectionState.Disconnected;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("InitiatingGame", OnInitiatingGame);
            _hubConnection.On<IDictionary<string, Game>>("NewGamesAvailable", OnNewGamesAvailableAsync);

            await _hubConnection.StartAsync();
        }

        async Task OnNewGamesAvailableAsync(IDictionary<string, Game> games)
        {
            await InvokeAsync(() =>
            {
                JoinableGames = games;
                StateHasChanged();
            });
        }

        public async Task StartGame()
        {
            if (PlayerName is not { Length: >= 3 })
            {
                return;
            }

            await _hubConnection.InvokeAsync(nameof(StartGame), PlayerName);
        }

        void OnInitiatingGame(string gameId)
        {
            NavigationManager.NavigateTo($"game/{gameId}");
        }

        async Task TryJoinGame(string gameId)
        {
            await _hubConnection.InvokeAsync("JoinGame", gameId, PlayerName);
        }

        public async ValueTask DisposeAsync()
            => await (_hubConnection?.DisposeAsync() ?? new ValueTask());
    }
}
