using System;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.Blazing.WarFleet
{
    static class Defaults
    {
        /// <summary>
        /// The board size with a 5x5 grid, and rows decorated as 'A'-'E' and columns 1-5.
        /// </summary>
        internal static HashSet<Position> FiveByFiveBoard { get; } =
            DefaultBoardPositions('A'..'E', 1..5).ToHashSet();

        /// <summary>
        /// The default board size is a 10x10 grid, with rows decorated as 'A'-'J' and columns 1-10.
        /// </summary>
        internal static HashSet<Position> TenByTenBoard { get; } =
            DefaultBoardPositions('A'..'J', 1..10).ToHashSet();

        /// <summary>
        /// The default board size is a 20x20 grid, with rows decorated as 'A'-'T' and columns 1-20.
        /// </summary>
        internal static HashSet<Position> TwentyByTwentyBoard { get; } =
            DefaultBoardPositions('A'..'T', 1..20).ToHashSet();

        static IEnumerable<Position> DefaultBoardPositions(Range rows, Range cols)
        {
            foreach (var rowVal in rows)
            {
                var row = (char)rowVal;
                foreach (var colVal in cols)
                {
                    var col = (byte)colVal;
                    yield return new(row, col);
                }
            }
        }
    }
}
