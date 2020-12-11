using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Hubs
{
    public class GameHub : Hub
    {
        readonly GameEngineService _gameEngineService;
        readonly GameHostService _gameHostService;

        public GameHub(GameEngineService gameEngineService, GameHostService gameHostService) =>
            (_gameEngineService, _gameHostService) = (gameEngineService, gameHostService);

        public async ValueTask StartGame(string playerName)
        {
            var game = await _gameHostService.StartGameAsync(playerName);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Caller.SendAsync("InitiatingGame", game.Id);
            await NewGamesAvailableAsync();
            await Clients.Group(game.Id).SendAsync("WaitingForOpponent", game.Id);
        }

        async Task NewGamesAvailableAsync()
        {
            var serverGames = await _gameHostService.GetJoinableGamesAsync();
            await Clients.All.SendAsync(
                "NewGamesAvailable", serverGames.ToDictionary(sg => sg.Id, sg => sg.Game));
        }

        public async ValueTask JoinGame(string gameId, string playerName)
        {
            var (game, player, joined) = await _gameHostService.TryJoinGameAsync(gameId, playerName);
            if (joined)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                await Clients.Group(game.Id).SendAsync("PlayerJoined", player);
            }
        }

        public async ValueTask PlaceShips(string gameId, string playerId, IEnumerable<Ship> ships)
        {
            var game = await _gameEngineService.PlacePlayerShipsAsync(gameId, playerId, ships);
            await Clients.Group(game.Id).SendAsync("GameUpdated", game);
        }

        public async ValueTask CallShot(string gameId, string playerId, Position shot)
        {
            var shotResult = await _gameEngineService.ProcessPlayerShotAsync(gameId, playerId, shot);
            var ((game, _), isHit, shipName) = shotResult;
            await Clients.Group(gameId).SendAsync("ShotFired",
                game.Result,
                isHit,
                shipName);
            
            if (game.Result.IsWinningResult())
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            }
            else
            {
                var (_, opponent) = game.GetPlayerAndOpponent(playerId);
                await Clients.Group(gameId).SendAsync("NextTurn", opponent.Id);
            }
        }
    }
}
