using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IEvangelist.Blazing.WarFleet.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class GameController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<object> Get() => new[] { new object() };
    }
}
