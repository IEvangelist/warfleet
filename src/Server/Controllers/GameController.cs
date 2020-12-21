using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Server.Controllers
{
    [ApiController, Route("api/games")]
    public class GameController : ControllerBase
    {
        [HttpGet("{gameId}")]
        public async ValueTask<Game> GetGame(
            [FromServices] IRepository<ServerGame> gameRepository,
            string gameId)
        {
            var serverGame = await gameRepository.GetAsync(gameId);
            return serverGame.Game;
        }

        [HttpGet("joinable")]
        public async ValueTask<IDictionary<string, Game>> GetJoinableGames(
            [FromServices] GameHostService gameHostService)
        {
            var serverGames = await gameHostService.GetJoinableGamesAsync();
            return serverGames.OrderBy(sg => sg.Started).ToDictionary(sg => sg.Id, sg => sg.Game);
        }
    }
}
