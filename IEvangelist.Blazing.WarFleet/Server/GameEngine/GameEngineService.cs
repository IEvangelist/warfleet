using IEvangelist.Blazing.WarFleet.Server.Extensions;
using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.Azure.CosmosRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet
{
    public class GameEngineService
    {
        readonly IRepository<Game> _gameRepository;

        public GameEngineService(IRepository<Game> gameRepository) =>
            _gameRepository = gameRepository;

        public async ValueTask<PlayerShotResult> ProcessPlayerShotAsync(
            string gameId,
            string playerId,
            Position shotPlacement)
        {
            var game = await _gameRepository.GetAsync(gameId);
            var (player, opponent) = game.GetPlayerAndOpponent(playerId);
            var (shipName, _) =
                opponent.PlacementBoard
                        .Ships
                        .Select(ship => (ship.Name, Occupancy: ship.GetShipOccupancy()))
                        .FirstOrDefault(ship => ship.Occupancy.Contains(shotPlacement));
            var isHit = shipName is { Length: > 0 };
            player.TrackingBoard.ShotsFired.Add(new(shotPlacement, isHit));

            var hasWonGame = player.HasWonGame(opponent.PlacementBoard.Ships);
            var gameResult = hasWonGame
                ? player.Id == game.PlayerOne.Id
                    ? GameResult.PlayerOneWins
                    : GameResult.PlayerTwoWins
                : GameResult.Active;

            game.Result = gameResult;

            return new PlayerShotResult(
                await _gameRepository.UpdateAsync(game),
                isHit,
                shipName);
        }

        public async ValueTask<Game> PlacePlayerShipsAsync(
            string gameId,
            string playerId,
            IList<Ship> ships)
        {
            var game = await _gameRepository.GetAsync(gameId);
            var (player, _) = game.GetPlayerAndOpponent(playerId);
            ships.ForEach(ship => player.PlacementBoard.Ships.Add(ship));

            return await _gameRepository.UpdateAsync(game);
        }
    }
}
