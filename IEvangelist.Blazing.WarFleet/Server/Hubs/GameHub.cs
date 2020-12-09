using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Hubs
{
    // Start game   -> "GameStarted" <game>, "WaitingForOpponent" <gameId>
    // Join game    -> "PlayerJoined" <player>
    // Place ships  -> "GameUpdated" <game>
    // Call shot    -> "ShotFired" <Result,IsHit,ShipName>
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
            await Clients.Caller.SendAsync("GameStarted", game);
            await Clients.Group(game.Id).SendAsync("WaitingForOpponent", game.Id);
            await Clients.Others.SendAsync("GameStarted", game);
        }

        public async ValueTask JoinGame(string gameId, string playerName)
        {
            var (game, player, joined) = await _gameHostService.TryJoinGameAsync(gameId, playerName);
            if (joined) await Clients.Group(game.Id).SendAsync("PlayerJoined", player);
        }

        public async ValueTask PlaceShips(string gameId, string playerId, IEnumerable<Ship> ships)
        {
            var game = await _gameEngineService.PlacePlayerShipsAsync(gameId, playerId, ships);
            await Clients.Group(game.Id).SendAsync("GameUpdated", game);
        }

        public async ValueTask CallShot(string gameId, string playerId, Position shot)
        {
            var result = await _gameEngineService.ProcessPlayerShotAsync(gameId, playerId, shot);
            var gameResult = result.Game.Result;
            await Clients.Group(result.Game.Id).SendAsync("ShotFired",
            new
            {
                GameResult = gameResult,
                result.IsHit,
                result.ShipName
            });
            
            if (gameResult.IsWinningResult())
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            }
        }
    }
}
