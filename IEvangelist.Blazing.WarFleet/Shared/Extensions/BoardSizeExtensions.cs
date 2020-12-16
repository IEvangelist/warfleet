using System.Collections.Generic;

namespace IEvangelist.Blazing.WarFleet
{
    public static class BoardSizeExtensions
    {
        public static HashSet<Position> ToPositionSet(this BoardSize boardSize) => boardSize switch
        {
            BoardSize.TwentyByTwenty => Defaults.TwentyByTwentyBoard,
            BoardSize.FiveByFive => Defaults.FiveByFiveBoard,

            _ => Defaults.TenByTenBoard
        };
    }
}