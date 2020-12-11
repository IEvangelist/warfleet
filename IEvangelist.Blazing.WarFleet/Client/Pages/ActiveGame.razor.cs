using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client.Pages
{
    public partial class ActiveGame : IAsyncDisposable
    {
        HubConnection? _hubConnection;
        bool _fireShotButtonEnabled;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public string GameId { get; set; } = null!;

        public string PlayerId { get; set; } = null!;
        public HubConnectionState ConnectionState =>
            _hubConnection?.State ?? HubConnectionState.Disconnected;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, Game>("GameUpdated", OnGameUpdatedAsync);
            _hubConnection.On<string, Player>("PlayerJoined", OnPlayerJoinedAsync);
            _hubConnection.On<GameResult, bool, string>("ShotFired", OnShotFiredAsync);
            _hubConnection.On<string>("NextTurn", OnNextTurnAsync);

            await _hubConnection.StartAsync();
        }

        async Task OnNextTurnAsync(string playerId)
        {
            await InvokeAsync(() => _fireShotButtonEnabled = PlayerId == playerId);
        }

        async Task OnGameUpdatedAsync(string gameId, Game game)
        {
            await Task.CompletedTask;
        }

        async Task OnPlayerJoinedAsync(string gameId, Player player)
        {
            await Task.CompletedTask;
        }

        async Task OnShotFiredAsync(GameResult result, bool isHit, string shipName)
        {
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
            => await (_hubConnection?.DisposeAsync() ?? new ValueTask());
    }
}
