using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet
{
    public static class PlayerExtensions
    {
        public static bool HasWonGame(this Player player, HashSet<Ship> opponentShips)
        {
            var shots = player.TrackingBoard.ShotsFired.Where(shot => shot.IsHit).Select(shot => shot.Shot).ToHashSet();
            return opponentShips.SelectMany(ship => ship.GetShipOccupancy()).All(shipPlacement => shots.Contains(shipPlacement));
        }
    }
}
