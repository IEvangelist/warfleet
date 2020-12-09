using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Hubs
{
    public class GameHub : Hub
    {
        readonly GameEngineService _gameEngineService;

        /*
        Isolated to a single game
        Wraps the GameEnginService
        The channel is between two players (might consider spectating mode?)
        */

        public GameHub(GameEngineService gameEngineService) =>
            _gameEngineService = gameEngineService;

        public ValueTask JoinGame(string gameId, string playerId)
        {
            return new ValueTask();
            //Clients.Group(gameId).SendAsync
        }

        public ValueTask SpectateGame(string gameId)
        {
            return new ValueTask();
        }
    }
}
