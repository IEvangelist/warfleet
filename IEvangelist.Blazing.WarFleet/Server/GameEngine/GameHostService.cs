using Microsoft.Azure.CosmosRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet
{
    public class GameHostService
    {
        private readonly IRepository<ServerGame> _gameRepository;

        public GameHostService(IRepository<ServerGame> gameRepository) =>
            _gameRepository = gameRepository;

        public ValueTask<ServerGame> StartGameAsync(string playerName) =>
            _gameRepository.CreateAsync(new ServerGame
            {
                Game = new()
                {
                    PlayerOne = playerName
                }
            });

        public ValueTask<IEnumerable<ServerGame>> GetJoinableGamesAsync() =>
            _gameRepository.GetAsync(sg => sg.Game.PlayerTwo == null);

        public async ValueTask<(ServerGame Game, Player? Player, bool Joined)> TryJoinGameAsync(
            string gameId, string playerName)
        {
            var serverGame = await _gameRepository.GetAsync(gameId);
            var game = serverGame.Game;
            var player = game.TryJoinGame(playerName);
            if (player is not null)
            {
                await _gameRepository.UpdateAsync(serverGame);
            }

            return (serverGame, player, player is not null);
        }
    }
}
