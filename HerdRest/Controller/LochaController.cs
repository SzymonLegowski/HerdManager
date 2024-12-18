using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LochaController : ControllerBase  // Użyj 'ControllerBase', nie 'Controller' w przypadku Web API
    {
        private readonly ILochaRepository _lochaRepository;

        public LochaController(ILochaRepository lochaRepository)
        {
            _lochaRepository = lochaRepository;
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateLocha([FromBody] Locha lochaCreate)
        {
            if(lochaCreate == null)
                return BadRequest(ModelState);

            var locha = _lochaRepository.GetLochy()
                .Where(l => l.NumerLochy == lochaCreate.NumerLochy && l.Status == 0)
                .FirstOrDefault();

            if(locha != null)
            {
                ModelState.AddModelError("", "Istnieje już aktywna locha o podanym numerze.");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_lochaRepository.CreateLocha(lochaCreate))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok("Dodano pomyślnie!");
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Locha>))]
        public IActionResult GetLochy()
        {
            var lochy = _lochaRepository.GetLochy().ToList();
            var dtos = _lochaRepository.MapToDtoList(lochy);

             if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dtos);
        }

        [HttpGet("{lochaId}")]
        [ProducesResponseType(200, Type = typeof(Locha))]
        [ProducesResponseType(400)]
        public IActionResult GetLocha(int lochaId)
        {
            if(!_lochaRepository.LochaExists(lochaId))
                return NotFound();
            
            var locha = _lochaRepository.GetLocha(lochaId);
            var dto = _lochaRepository.MapToDto(locha);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dto);
        }

        [HttpPut("{lochaId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateLocha(int lochaId, [FromBody]Locha updatedLocha)
        {
            if(updatedLocha == null)
                return BadRequest(ModelState);

            if(lochaId != updatedLocha.Id)
                return BadRequest(ModelState);

            if(!_lochaRepository.LochaExists(lochaId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();
            
            if(!_lochaRepository.UpdateLocha(updatedLocha))
                {
                    ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu zmian.");
                    return StatusCode(500, ModelState);
                }

            return Ok("Zapisano pomyślnie!");
        }
        
        [HttpDelete("{lochaId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteLocha(int lochaId)
        {
            if(!_lochaRepository.LochaExists(lochaId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var lochaToDelete = _lochaRepository.GetLocha(lochaId);

            if(!_lochaRepository.DeleteLocha(lochaToDelete))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy usuwaniu");
            }

            return Ok("Usunięto pomyślnie!");
        }
    
    }
}