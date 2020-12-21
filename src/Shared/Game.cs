namespace IEvangelist.Blazing.WarFleet
{
    public class Game
    {
        public Player PlayerOne { get; set; } = null!;

        public Player PlayerTwo { get; set; } = null!;

        public BoardSize BoardSize { get; set; } = BoardSize.TenByTen;

        public GameResult Result { get; set; } = GameResult.NotYetStarted;

        public bool PlayersReady =>
            (PlayerOne?.ShipsPlaced ?? false) &&
            (PlayerTwo?.ShipsPlaced ?? false);

        public Player? TryJoinGame(string playerName) => this switch
        {
            { PlayerOne: null } => PlayerOne = playerName,
            { PlayerTwo: null } => PlayerTwo = playerName,
            _ => default
        };
    }
}
