using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet
{
    public static class ShipExtensions
    {
        public static HashSet<Position> GetShipOccupancy(this Ship ship)
        {
            var isHorizontal = ship.Alignment == ShipAlignment.Horizontal;
            var (row, col) = ship.TopLeftPosition ?? new Position('A', 1);
            return Enumerable.Range((int)(isHorizontal ? col : row), ship.Size)
                .Select(value => new Position((char)(isHorizontal ? row : value), (byte)(isHorizontal ? value : col)))
                .ToHashSet();
        }
    }
}
