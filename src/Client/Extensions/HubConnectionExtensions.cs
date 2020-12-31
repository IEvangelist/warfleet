using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client.Extensions
{
    static class HubConnectionExtensions
    {
        internal static HubConnection OnGameUpdated(
            this HubConnection hubConnection, Func<string, Game, Task> handler)
        {
            _ = hubConnection.On("GameUpdated", handler);
            return hubConnection;
        }

        internal static HubConnection OnGameLogUpdated(
            this HubConnection hubConnection, Func<string, Task> handler)
        {
            _ = hubConnection.On("GameLogUpdated", handler);
            return hubConnection;
        }

        internal static HubConnection OnPlayerJoined(
            this HubConnection hubConnection, Func<Player, Task> handler)
        {
            _ = hubConnection.On("PlayerJoined", handler);
            return hubConnection;
        }

        internal static HubConnection OnNextTurn(
            this HubConnection hubConnection, Func<string, Task> handler)
        {
            _ = hubConnection.On("NextTurn", handler);
            return hubConnection;
        }

        internal static HubConnection OnShotFired(
            this HubConnection hubConnection, Func<PlayerShot, Task> handler)
        {
            _ = hubConnection.On("ShotFired", handler);
            return hubConnection;
        }

        internal static HubConnection OnJoinableGamesUpdated(
            this HubConnection hubConnection, Func<IDictionary<string, Game>, Task> handler)
        {
            _ = hubConnection.On("JoinableGamesUpdated", handler);
            return hubConnection;
        }

        internal static Task StartGame(
            this HubConnection hubConnection, string gameId, BoardSize boardSize, string playerName) =>
            hubConnection.InvokeAsync(nameof(StartGame), gameId, boardSize, playerName);

        internal static Task JoinGame(
            this HubConnection hubConnection, string gameId, string playerName) =>
            hubConnection.InvokeAsync(nameof(JoinGame), gameId, playerName);

        internal static Task LeaveGame(
            this HubConnection hubConnection, string gameId) =>
            hubConnection.InvokeAsync(nameof(LeaveGame), gameId);

        internal static Task CallShot(
            this HubConnection hubConnection, string gameId, string playerId, Position shot) =>
            hubConnection.InvokeAsync(nameof(CallShot), gameId, playerId, shot);

        internal static Task PlaceShips(
            this HubConnection hubConnection, string gameId, string playerId, HashSet<Ship> placedShips) =>
            hubConnection.InvokeAsync(nameof(PlaceShips), gameId, playerId, placedShips);
    }
}
