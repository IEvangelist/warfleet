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
        HubConnection? _hubConnection;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public HttpClient Http { get; set; } = null!;

        public bool HasValidName => PlayerName is { Length: >= 3 };
        public string PlayerName { get; set; } = null!;
        public IDictionary<string, Game>? JoinableGames { get; set; } = null!;
        public HubConnectionState ConnectionState =>
            _hubConnection?.State ?? HubConnectionState.Disconnected;

        protected override async Task OnInitializedAsync()
        {
            JoinableGames =
                await Http.GetFromJsonAsync<IDictionary<string, Game>>("api/games/joinable");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<IDictionary<string, Game>>(
                "JoinableGamesUpdated", OnJoinableGamesUpdatedAsync);

            await _hubConnection.StartAsync();
        }

        async Task OnJoinableGamesUpdatedAsync(IDictionary<string, Game> games) =>
            await InvokeAsync(() =>
            {
                JoinableGames = games;
                StateHasChanged();
            });

        public void StartGame(BoardSize size)
        {
            if (PlayerName is not { Length: >= 3 })
            {
                return;
            }

            NavigationManager.NavigateTo($"game/{Guid.NewGuid()}?newGame=true&playerName={PlayerName}&boardSize={size}");
        }

        public void TryJoinGame(string gameId)
        {
            if (PlayerName is not { Length: >= 3 })
            {
                return;
            }

            NavigationManager.NavigateTo($"game/{gameId}?playerName={PlayerName}");
        }

        public async ValueTask DisposeAsync()
            => await (_hubConnection?.DisposeAsync() ?? new ValueTask());
    }
}
