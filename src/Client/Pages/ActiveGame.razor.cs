using IEvangelist.Blazing.WarFleet.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class ActiveGame : IAsyncDisposable
    {
        readonly List<string> _gameLog = new();
        readonly HashSet<Ship> _placedShips = new();

        Game? _game;
        HubConnection? _hubConnection;

        BoardSize _boardSize;
        List<Ship> _availableShips = null!;

        bool _fireShotButtonEnabled;
        bool _isNewGame;
        string _playerName = null!;
        string _playerId = null!;

        [Inject]
        public ILogger<ActiveGame> Log { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient Http { get; set; } = null!;

        [Parameter]
        public string GameId { get; set; } = null!;

        public HubConnectionState ConnectionState =>
            _hubConnection?.State ?? HubConnectionState.Disconnected;

        public Ship DraggingShip { get; set; } = null!;

        public Position DraggingShipPosition { get; set; } = null!;

        public HashSet<PlayerMove> ShotsFired { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.TryGetQueryString("newGame", out _isNewGame);
            NavigationManager.TryGetQueryString("playerName", out _playerName);
            NavigationManager.TryGetQueryString("boardSize", out _boardSize);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, Game>("GameUpdated", OnGameUpdatedAsync);
            _hubConnection.On<string>("GameLogUpdated", OnGameLogUpdatedAsync);
            _hubConnection.On<Player>("PlayerJoined", OnPlayerJoinedAsync);
            _hubConnection.On<GameResult, Position, bool, string>("ShotFired", OnShotFiredAsync);
            _hubConnection.On<string>("NextTurn", OnNextTurnAsync);

            await _hubConnection.StartAsync();

            var startOrJoinGameTask = _isNewGame
                ? _hubConnection.InvokeAsync("StartGame", GameId, _boardSize, _playerName)
                : _hubConnection.InvokeAsync("JoinGame", GameId, _playerName);

            await startOrJoinGameTask;
        }

        async Task PlacePlayerShips() =>
            await _hubConnection.InvokeAsync("PlaceShips", GameId, _playerId, _placedShips);

        async Task OnShipPlaced(Ship ship) =>
            await InvokeAsync(() =>
            {
                _placedShips.Add(ship);
                _availableShips?.Remove(DraggingShip);

                StateHasChanged();
            });

        async Task OnGameLogUpdatedAsync(string message) =>
            await InvokeAsync(() =>
            {
                _gameLog.Add(message);
                StateHasChanged();
            });

        async Task OnNextTurnAsync(string playerId) =>
            await InvokeAsync(() => _fireShotButtonEnabled = _playerId == playerId);

        async Task OnGameUpdatedAsync(string playerId, Game game) =>
            await InvokeAsync(() =>
            {
                _game = game;
                _availableShips = _game.BoardSize.ToShipSet();
                _playerId = playerId;

                StateHasChanged();
            });

        async Task OnPlayerJoinedAsync(Player player) =>
            await InvokeAsync(() => StateHasChanged());

        async Task OnShotCalled(Position shot) =>
            await _hubConnection.InvokeAsync("CallShot", GameId, _playerId, shot);

        async Task OnShotFiredAsync(
            GameResult result, Position shot, bool isHit, string shipName) =>
            await InvokeAsync(() =>
            {
                if (_game is not null)
                {
                    _ = ShotsFired.Add(new(shot, isHit));

                    var (player, _) = _game.GetPlayerAndOpponent(_playerId);
                    foreach (var playerShot in player?.ShotsFired ?? Enumerable.Empty<PlayerMove>().ToHashSet())
                    {
                        _ = ShotsFired.Add(playerShot);
                    }

                    StateHasChanged();
                }
            });

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is null)
            {
                return;
            }

            await _hubConnection.InvokeAsync("LeaveGame", GameId);
            await _hubConnection.DisposeAsync();
        }

        string OpponentName()
        {
            if (_game is not null)
            {
                var (_, opponent) = _game.GetPlayerAndOpponent(_playerId);
                return opponent?.Name ?? "Unknown";
            }

            return "Unknown";
        }
    }
}
