namespace IEvangelist.Blazing.WarFleet
{
    public static class GameExtensions
    {
        public static (Player? Player, Player? Opponent) GetPlayerAndOpponent(
            this Game game, string playerId)
        {
            if (game is { PlayerOne: null } or { PlayerTwo: null })
            {
                return (null, null);
            }

            var player = game.PlayerOne.Id == playerId ? game.PlayerOne : game.PlayerTwo.Id == playerId ? game.PlayerTwo : null;
            var opponent = game.PlayerTwo.Id != playerId ? game.PlayerTwo : game.PlayerOne.Id != playerId ? game.PlayerOne : null;

            return (player, opponent);
        }
    }
}