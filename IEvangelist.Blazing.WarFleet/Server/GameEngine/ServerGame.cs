using Microsoft.Azure.CosmosRepository;
using System;

namespace IEvangelist.Blazing.WarFleet
{
    public class ServerGame : Item
    {
        public DateTime Started { get; set; } = DateTime.UtcNow;

        public DateTime? Ended { get; set; } = null;

        public Game Game { get; set; } = null!;

        public void Deconstruct(out Game game, out string id) =>
            (game, id) = (Game, Id);
    }
}
