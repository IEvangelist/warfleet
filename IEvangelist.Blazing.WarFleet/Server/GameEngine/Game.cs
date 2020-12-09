using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.Azure.CosmosRepository;

namespace IEvangelist.Blazing.WarFleet
{
    public class Game : Item
    {
        public Player PlayerOne { get; set; } = null!;

        public Player PlayerTwo { get; set; } = null!;

        public GameResult Result { get; set; }

        public bool PlayersReady =>
            (PlayerOne?.ShipsPlaced ?? false) &&
            (PlayerTwo?.ShipsPlaced ?? false);

        internal Player? TryJoinGame(string playerName) => this switch
        {
            { PlayerOne: null } => PlayerOne = playerName,
            { PlayerTwo: null } => PlayerTwo = playerName,
            _ => null
        };
    }
}
