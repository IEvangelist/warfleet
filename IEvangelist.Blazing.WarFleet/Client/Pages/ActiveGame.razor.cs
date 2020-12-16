using IEvangelist.Blazing.WarFleet.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class ActiveGame : IAsyncDisposable
    {
        readonly List<string> _gameLog = new();

        Game? _game;
        HubConnection? _hubConnection;
        BoardSize _boardSize;
        bool _fireShotButtonEnabled;
        bool _isNewGame;
        string _playerName = null!;

        [Inject]
        public ILogger<ActiveGame> Log { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient Http { get; set; } = null!;

        [Parameter]
        public string GameId { get; set; } = null!;

        public string PlayerId { get; set; } = null!;
        public HubConnectionState ConnectionState =>
            _hubConnection?.State ?? HubConnectionState.Disconnected;

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.TryGetQueryString("newGame", out _isNewGame);
            NavigationManager.TryGetQueryString("playerName", out _playerName);
            NavigationManager.TryGetQueryString("boardSize", out _boardSize);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<Game>("GameUpdated", OnGameUpdatedAsync);
            _hubConnection.On<string>("GameLogUpdated", OnGameLogUpdatedAsync);
            _hubConnection.On<string, Player>("PlayerJoined", OnPlayerJoinedAsync);
            _hubConnection.On<GameResult, bool, string>("ShotFired", OnShotFiredAsync);
            _hubConnection.On<string>("NextTurn", OnNextTurnAsync);

            await _hubConnection.StartAsync();

            var startOrJoinGameTask = _isNewGame
                ? _hubConnection.InvokeAsync("StartGame", GameId, _boardSize, _playerName)
                : _hubConnection.InvokeAsync("JoinGame", GameId, _playerName);

            await startOrJoinGameTask;
        }

        async Task OnGameLogUpdatedAsync(string message) =>
            await InvokeAsync(() =>
            {
                _gameLog.Add(message);
                StateHasChanged();
            });

        async Task OnNextTurnAsync(string playerId)
        {
            await InvokeAsync(() => _fireShotButtonEnabled = PlayerId == playerId);
        }

        async Task OnGameUpdatedAsync(Game game) =>
            await InvokeAsync(() =>
            {
                _game = game;
                StateHasChanged();
            });

        async Task OnPlayerJoinedAsync(string gameId, Player player)
        {
            await Task.CompletedTask;
        }

        async Task OnShotFiredAsync(GameResult result, bool isHit, string shipName)
        {
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is null)
            {
                return;
            }

            await _hubConnection.InvokeAsync("LeaveGame", GameId);
            await _hubConnection.DisposeAsync();
        }
    }
}
