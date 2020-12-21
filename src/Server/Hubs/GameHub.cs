using IEvangelist.Blazing.WarFleet.Server.Extensions;
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

        public async ValueTask StartGame(string gameId, BoardSize size, string playerName)
        {
            var game = await _gameHostService.StartGameAsync(gameId, size, playerName);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            await Clients.Caller.SendAsync("GameUpdated", game);
            await Clients.Group(gameId).SendAsync("GameLogUpdated", $"{playerName} started game ({gameId}).");

            await JoinableGamesUpdatedAsync();
        }

        async Task JoinableGamesUpdatedAsync()
        {
            var serverGames = await _gameHostService.GetJoinableGamesAsync();
            await Clients.All.SendAsync(
                "JoinableGamesUpdated", serverGames.OrderBy(sg => sg.Started).ToDictionary(sg => sg.Id, sg => sg.Game));
        }

        public async ValueTask JoinGame(string gameId, string playerName)
        {
            var (game, player, joined) = await _gameHostService.TryJoinGameAsync(gameId, playerName);
            if (joined)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

                await Clients.Caller.SendAsync("GameUpdated", game);
                await Clients.Group(gameId).SendAsync("PlayerJoined", player);
                await Clients.Group(gameId).SendAsync("GameLogUpdated", $"{playerName} joined game ({gameId}).");
            }
        }

        public async ValueTask PlaceShips(string gameId, string playerId, IEnumerable<Ship> ships)
        {
            var game = await _gameEngineService.PlacePlayerShipsAsync(gameId, playerId, ships);

            var playerAndOpponent = game.Game.GetPlayerAndOpponent(playerId);

            await Clients.Group(game.Id).SendAsync(
                "GameLogUpdated", $"{playerAndOpponent.Player.Name} has placed their ships and is ready.");

            await Clients.Group(game.Id).SendAsync("GameUpdated", game);

            if (game.Game.PlayersReady)
            {
                var playerToStart = playerAndOpponent.Random();
                await Clients.Group(gameId).SendAsync("NextTurn", playerToStart.Id);
            }
        }

        public async ValueTask CallShot(string gameId, string playerId, Position shot)
        {
            var shotResult = await _gameEngineService.ProcessPlayerShotAsync(gameId, playerId, shot);
            var ((game, _), isHit, shipName) = shotResult;
            await Clients.Group(gameId).SendAsync("ShotFired",
                game.Result,
                isHit,
                shipName);

            var (player, opponent) = game.GetPlayerAndOpponent(playerId);
            await Clients.Group(gameId).SendAsync(
                "GameLogUpdated", $"{player.Name} fires on {shot}.{(isHit ? $"Hitting {opponent.Name} {shipName}!" : "Miss!")}");

            if (game.Result.IsWinningResult())
            {
                await Clients.Group(gameId).SendAsync("GameLogUpdated", $"{player.Name} wins!");
                await LeaveGame(gameId);
            }
            else
            {
                await Clients.Group(gameId).SendAsync("NextTurn", opponent.Id);
            }
        }

        public async ValueTask LeaveGame(string gameId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}
