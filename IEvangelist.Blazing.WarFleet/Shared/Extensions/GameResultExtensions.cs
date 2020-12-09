using IEvangelist.Blazing.WarFleet.Shared;

namespace IEvangelist.Blazing.WarFleet
{
    public static class GameResultExtensions
    {
        public static bool IsWinningResult(this GameResult result)
            => result == GameResult.PlayerOneWins
            || result == GameResult.PlayerTwoWins;
    }
}