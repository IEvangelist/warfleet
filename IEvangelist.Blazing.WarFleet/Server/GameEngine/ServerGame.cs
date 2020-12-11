using Microsoft.Azure.CosmosRepository;

namespace IEvangelist.Blazing.WarFleet
{
    public class ServerGame : Item
    {
        public Game Game { get; set; } = null!;

        public void Deconstruct(out Game game, out string id) =>
            (game, id) = (Game, Id);
    }
}
