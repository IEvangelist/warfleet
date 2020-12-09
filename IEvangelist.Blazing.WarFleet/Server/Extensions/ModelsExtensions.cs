using IEvangelist.Blazing.WarFleet.Shared;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet.Server.Extensions
{
    public static class ModelsExtensions
    {
        public static HashSet<Position> GetShipOccupancy(this Ship ship)
        {
            var isHorizontal = ship.Alignment == ShipAlignment.Horizontal;
            var (row, col) = ship.TopLeftPosition ?? new Position('A', 1);
            return Enumerable.Range((int)(isHorizontal ? col : row), ship.Size)
                .Select(value => new Position((char)(isHorizontal ? row : value), (byte)(isHorizontal ? value : col)))
                .ToHashSet();
        }

        public static (Player Player, Player Opponent) GetPlayerAndOpponent(this Game game, string playerId)
        {
            var player = game.PlayerOne.Id == playerId ? game.PlayerOne : game.PlayerTwo;
            var opponent = game.PlayerOne.Id == playerId ? game.PlayerTwo : game.PlayerOne;

            return (player, opponent);
        }

        public static bool HasWonGame(this Player player, HashSet<Ship> opponentShips)
        {
            var shots = player.TrackingBoard.ShotsFired.Where(shot => shot.IsHit).Select(shot => shot.Shot).ToHashSet();
            return opponentShips.SelectMany(ship => ship.GetShipOccupancy()).All(shipPlacement => shots.Contains(shipPlacement));
        }
    }
}
