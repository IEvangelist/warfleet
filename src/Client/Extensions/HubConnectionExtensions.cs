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
            _ = hubConnection.On<string, Game>("GameUpdated", handler);
            return hubConnection;
        }

        internal static HubConnection OnGameLogUpdated(
            this HubConnection hubConnection, Func<string, Task> handler)
        {
            _ = hubConnection.On<string>("GameLogUpdated", handler);
            return hubConnection;
        }

        internal static HubConnection OnPlayerJoined(
            this HubConnection hubConnection, Func<Player, Task> handler)
        {
            _ = hubConnection.On<Player>("PlayerJoined", handler);
            return hubConnection;
        }

        internal static HubConnection OnNextTurn(
            this HubConnection hubConnection, Func<string, Task> handler)
        {
            _ = hubConnection.On<string>("NextTurn", handler);
            return hubConnection;
        }

        internal static HubConnection OnShotFired(
            this HubConnection hubConnection, Func<PlayerShot, Task> handler)
        {
            _ = hubConnection.On<PlayerShot>("ShotFired", handler);
            return hubConnection;
        }

        internal static HubConnection OnJoinableGamesUpdated(
            this HubConnection hubConnection, Func<IDictionary<string, Game>, Task> handler)
        {
            _ = hubConnection.On<IDictionary<string, Game>>("JoinableGamesUpdated", handler);
            return hubConnection;
        }
    }
}
