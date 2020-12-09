namespace IEvangelist.Blazing.WarFleet.Shared
{
    public static class GameResultExtensions
    {
        public static bool IsWinningResult(this GameResult result)
            => result == GameResult.PlayerOneWins
            || result == GameResult.PlayerTwoWins;
    }
}