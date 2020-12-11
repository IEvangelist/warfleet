using Microsoft.Azure.CosmosRepository;
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
                await _gameRepository.UpdateAsync(serverGame),
                isHit,
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
            ships.ForEach(ship => player.PlacementBoard.Ships.Add(ship));

            return await _gameRepository.UpdateAsync(serverGame);
        }
    }
}
