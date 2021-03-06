﻿using IEvangelist.Blazing.WarFleet.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using IEvangelist.Blazing.WarFleet.Shared;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class ActiveGame : IAsyncDisposable
    {
        readonly List<string> _gameLog = new();
        readonly HashSet<Ship> _placedShips = new();

        Game? _game;
        HubConnection? _serverConnection;
        BoardSize _boardSize;
        List<Ship> _availableShips = null!;
        bool _userClickedSetShips = false;
        bool _isNewGame;
        string _opponentName = null!;
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
            _serverConnection?.State ?? HubConnectionState.Disconnected;

        public Ship DraggingShip { get; set; } = null!;
        public Position DraggingShipPosition { get; set; } = null!;
        public HashSet<PlayerMove> ShotsFired { get; set; } = new();

        public bool IsYourBoardDisabled { get; set; } = false;
        public bool IsTrackingBoardDisabled { get; set; } = true;
        public bool ShipsPlaced => (_placedShips.Count, _game!.BoardSize) switch
        {
            (3, BoardSize.FiveByFive) => true,
            (5, BoardSize.TenByTen) => true,
            (10, BoardSize.TwentyByTwenty) => true,

            _ => false
        };

        protected override async Task OnInitializedAsync()
        {
            _ = NavigationManager.TryGetQueryString("newGame", out _isNewGame);
            _ = NavigationManager.TryGetQueryString("playerName", out _playerName);
            _ = NavigationManager.TryGetQueryString("boardSize", out _boardSize);

            _serverConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            await _serverConnection.OnGameUpdated(OnGameUpdatedAsync)
                .OnGameLogUpdated(OnGameLogUpdatedAsync)
                .OnPlayerJoined(OnPlayerJoinedAsync)
                .OnShotFired(OnShotFiredAsync)
                .OnNextTurn(OnNextTurnAsync)
                .StartAsync();

            await (_isNewGame
                ? _serverConnection.StartGame(GameId, _boardSize, _playerName)
                : _serverConnection.JoinGame(GameId, _playerName));
        }

        async Task PlacePlayerShips()
        {
            _userClickedSetShips = true;
            await _serverConnection!.PlaceShips(GameId, _playerId, _placedShips);
        }

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

        async Task OnNextTurnAsync(string nextPlayerId) =>
            await InvokeAsync(() =>
            {
                IsYourBoardDisabled = true;
                IsTrackingBoardDisabled = _playerId != nextPlayerId;

                StateHasChanged();
            });

        async Task OnGameUpdatedAsync(string playerId, Game game) =>
            await InvokeAsync(() =>
            {
                _game = game;
                _availableShips = _game.BoardSize.ToShipSet();
                _playerId = playerId;

                var (_, opponent) = _game.GetPlayerAndOpponent(_playerId);
                _opponentName = opponent?.Name ?? "(Waiting for opponent)";

                StateHasChanged();
            });

        async Task OnPlayerJoinedAsync(Player player) =>
            await InvokeAsync(() =>
            {
                if (player is not null)
                {
                    _opponentName = player.Name;

                    StateHasChanged();
                }
            });

        async Task OnCallShot(Position shot) =>
            await _serverConnection!.CallShot(GameId, _playerId, shot);

        async Task OnShotFiredAsync(
            PlayerShot calledShot) =>
            await InvokeAsync(() =>
            {
                if (_game is not null && calledShot.PlayerId == _playerId)
                {
                    _ = ShotsFired.Add(new(calledShot.Shot, calledShot.IsHit));

                    var (player, _) = _game.GetPlayerAndOpponent(_playerId);
                    foreach (var playerShot in player?.ShotsFired ?? Enumerable.Empty<PlayerMove>().ToHashSet())
                    {
                        _ = ShotsFired.Add(playerShot);
                    }

                    StateHasChanged();
                }
            });

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_serverConnection is null)
            {
                return;
            }

            await _serverConnection.LeaveGame(GameId);
            await _serverConnection.DisposeAsync();
            
            _serverConnection = null;
        }
    }
}
