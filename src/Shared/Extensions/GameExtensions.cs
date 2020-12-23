namespace IEvangelist.Blazing.WarFleet
{
    public static class GameExtensions
    {
        public static (Player? Player, Player? Opponent) GetPlayerAndOpponent(this Game g, string playerId)
        {
            if (g is { PlayerOne: null } or { PlayerTwo: null })
            {
                return (null, null);
            }

            var player = g.PlayerOne.Id == playerId ? g.PlayerOne : g.PlayerTwo.Id == playerId ? g.PlayerTwo : null;
            var opponent = g.PlayerTwo.Id == playerId ? g.PlayerTwo : g.PlayerOne.Id == playerId ? g.PlayerOne : null;

            return (player, opponent);
        }
    }
}