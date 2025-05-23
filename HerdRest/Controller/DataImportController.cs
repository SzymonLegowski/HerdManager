using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataImportController : ControllerBase
    {

        [HttpPost]
        public IActionResult ImportData([FromBody]string data)
        {
            Console.WriteLine(data);   
            return Ok(data);
        }
    }
}
