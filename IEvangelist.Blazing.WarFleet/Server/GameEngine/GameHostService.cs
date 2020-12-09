using IEvangelist.Blazing.WarFleet.Shared;
using Microsoft.Azure.CosmosRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet
{
    public class GameHostService
    {
        private readonly IRepository<Game> _gameRepository;

        public GameHostService(IRepository<Game> gameRepository) =>
            _gameRepository = gameRepository;

        public ValueTask<Game> StartGameAsync(string playerName) =>
            _gameRepository.CreateAsync(new Game
            {
                PlayerOne = playerName
            });

        public ValueTask<IEnumerable<Game>> GetJoinableGamesAsync() =>
            _gameRepository.GetAsync(game => game.PlayerTwo == null);

        public async ValueTask<(Game Game, Player? Player, bool Joined)> TryJoinGameAsync(string gameId, string playerName)
        {
            var game = await _gameRepository.GetAsync(gameId);
            var player = game.TryJoinGame(playerName);
            if (player is not null)
            {
                await _gameRepository.UpdateAsync(game);
            }

            return (game, player, player is not null);
        }
    }
}
