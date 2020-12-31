using IEvangelist.Blazing.WarFleet.Server.Extensions;
using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        readonly GameEngineService _gameEngineService;
        readonly GameHostService _gameHostService;

        public GameHub(GameEngineService gameEngineService, GameHostService gameHostService) =>
            (_gameEngineService, _gameHostService) = (gameEngineService, gameHostService);

        public async ValueTask StartGame(string gameId, BoardSize size, string playerName)
        {
            var serverGame = await _gameHostService.StartGameAsync(gameId, size, playerName);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            await Clients.Caller.GameUpdated(serverGame.Game.PlayerOne.Id, serverGame.Game);
            await Clients.Group(gameId).GameLogUpdated($"{playerName} started game ({gameId}).");

            await JoinableGamesUpdatedAsync();
        }

        async Task JoinableGamesUpdatedAsync()
        {
            var serverGames = await _gameHostService.GetJoinableGamesAsync();
            await Clients.All.JoinableGamesUpdated(
                serverGames.OrderBy(sg => sg.Started).ToDictionary(sg => sg.Id, sg => sg.Game));
        }

        public async ValueTask JoinGame(string gameId, string playerName)
        {
            var (serverGame, player, joined) = await _gameHostService.TryJoinGameAsync(gameId, playerName);
            if (joined && player is not null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

                await Clients.Caller.GameUpdated(player.Id, serverGame.Game);
                await Clients.Group(gameId).PlayerJoined(player);
                await Clients.Group(gameId).GameLogUpdated($"{playerName} joined game ({gameId}).");
            }
        }

        public async ValueTask PlaceShips(string gameId, string playerId, IEnumerable<Ship> ships)
        {
            var serverGame = await _gameEngineService.PlacePlayerShipsAsync(gameId, playerId, ships);
            var playerAndOpponent = serverGame.Game.GetPlayerAndOpponent(playerId);

            if (playerAndOpponent is not { Player: null } and not { Opponent: null })
            {
                await Clients.Group(serverGame.Id).GameLogUpdated(
                    $"{playerAndOpponent.Player.Name} has placed their ships and is ready.");

                if (serverGame.Game.PlayersReady)
                {
                    var playerToStart = playerAndOpponent.Random();
                    if (playerToStart is not null)
                    {
                        await Clients.Group(gameId).NextTurn(playerToStart.Id);
                        await Clients.Group(gameId).GameLogUpdated(
                            $"{playerToStart.Name} goes first.");
                    }
                }
            }
        }

        public async ValueTask CallShot(string gameId, string playerId, Position shot)
        {
            var shotResult = await _gameEngineService.ProcessPlayerShotAsync(gameId, playerId, shot);
            var ((game, _), isHit, isSunk, shipName) = shotResult;
            await Clients.Group(gameId).ShotFired(new(
                game.Result,
                playerId,
                shot,
                isHit,
                isSunk,
                shipName));

            var (player, opponent) = game.GetPlayerAndOpponent(playerId);
            if (player is not null && opponent is not null)
            {
                var isHitMessage = isHit ? $"Hitting {opponent.Name} {shipName}!" : "Miss!";
                var isSunkMessage = isSunk ? $"{player.Name} sunk {opponent.Name}'s {shipName}." : "";
                if (game.Result.IsWinningResult())
                {
                    await Clients.Group(gameId).GameLogUpdated($"{player.Name} wins!");
                }
                else
                {
                    await Clients.Group(gameId).NextTurn(opponent.Id);
                }

                await Clients.Group(gameId).GameLogUpdated(
                    $"{player.Name} fires on {shot}. {isHitMessage} {isSunkMessage}");
            }
        }

        public async ValueTask LeaveGame(string gameId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}
