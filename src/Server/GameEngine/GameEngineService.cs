using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet
{
    public class GameEngineService
    {
        readonly IRepository<ServerGame> _gameRepository;

        public GameEngineService(IRepository<ServerGame> gameRepository) =>
            _gameRepository = gameRepository;

        public async ValueTask<PlayerShotResult> ProcessPlayerShotAsync(
            string gameId,
            string playerId,
            Position shotPlacement)
        {
            var serverGame = await _gameRepository.GetAsync(gameId);
            var game = serverGame.Game;
            var (player, opponent) = game.GetPlayerAndOpponent(playerId);
            if (player is null || opponent is null)
            {
                return new(serverGame, false, false, null);
            }

            var shipOccupancies =
                opponent.Ships
                        .Select(ship => (ship.Name, ship.Size, Occupancy: ship.GetShipOccupancy()));

            var (shipName, _, _) =
                shipOccupancies
                    .FirstOrDefault(ship => ship.Occupancy.Contains(shotPlacement));

            var isHit = shipName is { Length: > 0 };
            player.ShotsFired.Add(new(shotPlacement, isHit));
            var playerHits = player.ShotsFired.Where(shot => shot.IsHit).Select(shot => shot.Shot);
            var isSunk = isHit
                && shipOccupancies.Where(ship => ship.Name == shipName)
                    .SelectMany(ship => ship.Occupancy)
                    .All(shot => playerHits is not null && playerHits.Contains(shot));

            var hasWonGame = player.HasWonGame(opponent.Ships);
            var gameResult = hasWonGame
                ? player.Id == game.PlayerOne.Id
                    ? GameResult.PlayerOneWins
                    : GameResult.PlayerTwoWins
                : GameResult.Active;

            game.Result = gameResult;
            if (gameResult.IsWinningResult() && !serverGame.Ended.HasValue)
            {
                serverGame.Ended = DateTime.UtcNow;
            }

            return new(
                await _gameRepository.UpdateAsync(serverGame),
                isHit,
                isSunk,
                shipName);
        }

        public async ValueTask<ServerGame> PlacePlayerShipsAsync(
            string gameId,
            string playerId,
            IEnumerable<Ship> ships)
        {
            var serverGame = await _gameRepository.GetAsync(gameId);
            var game = serverGame.Game;
            var (player, _) = game.GetPlayerAndOpponent(playerId);
            if (player is not null)
            {
                ships.ForEach(ship => player.Ships.Add(ship));
            }

            return await _gameRepository.UpdateAsync(serverGame);
        }
    }
}
