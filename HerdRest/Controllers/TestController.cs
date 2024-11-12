using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase  // Użyj 'ControllerBase', nie 'Controller' w przypadku Web API
    {
        // GET: api/test
        [HttpGet]
        public ActionResult<string> GetHelloWorld()
        {
            return "Czy aby na pewno???"; // Zwraca prosty tekst
        }
    }
}
