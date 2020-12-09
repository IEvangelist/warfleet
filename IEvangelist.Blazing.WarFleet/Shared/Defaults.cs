using IEvangelist.Blazing.WarFleet.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet.Shared
{
    public class Defaults
    {
        /// <summary>
        /// The default board size is a 10x10 grid, with rows decorated as 'A'-'J' and columns 1-10.
        /// </summary>
        public static HashSet<Position> GameBoard => DefaultBoardPositions().ToHashSet();

        static IEnumerable<Position> DefaultBoardPositions()
        {
            foreach (var rowVal in 'A'..'J')
            {
                var row = (char)rowVal;
                foreach (var colVal in 1..10)
                {
                    var col = (byte)colVal;
                    yield return new(row, col);
                }
            }
        }
    }
}
