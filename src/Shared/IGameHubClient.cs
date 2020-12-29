using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Shared
{
    public interface IGameHubClient
    {
        Task GameUpdated(string playerId, Game game);

        Task GameLogUpdated(string gameLogMessage);

        Task PlayerJoined(Player player);

        Task NextTurn(string nextPlayerId);

        Task ShotFired(PlayerShot shotResult);

        Task JoinableGamesUpdated(IDictionary<string, Game> games);
    }
}
