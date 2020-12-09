using IEvangelist.Blazing.WarFleet.Server.GameEngine;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Hubs
{
    public class GameServerHub : Hub
    {
        readonly GameHostService _gameHostService;

        /*
        Listing of active games
        Joinable games
        Games in progress (maybe add spectating mode?)
        Leader board
        Psuedo-lobby?
        Expose chat?
        */

        public GameServerHub(GameHostService gameHostService) =>
            _gameHostService = gameHostService;

        public async ValueTask JoinGame(string gameId, string playerName)
        {
            _gameHostService.TryJoinGameAsync(gameId, playerName, )
        }
    }
}
