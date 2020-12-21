namespace IEvangelist.Blazing.WarFleet
{
    public static class GameExtensions
    {
        public static (Player Player, Player Opponent) GetPlayerAndOpponent(this Game game, string playerId)
        {
            var player = game.PlayerOne.Id == playerId ? game.PlayerOne : game.PlayerTwo;
            var opponent = game.PlayerOne.Id == playerId ? game.PlayerTwo : game.PlayerOne;

            return (player, opponent);
        }
    }
}