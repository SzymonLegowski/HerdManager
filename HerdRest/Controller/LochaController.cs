using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.Enums;
using Microsoft.AspNetCore.Mvc;
using HerdRest.PublicClasses;

namespace HerdRest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LochaController(ILochaRepository lochaRepository) : ControllerBase  // Użyj 'ControllerBase', nie 'Controller' w przypadku Web API
    {
        private readonly ILochaRepository _lochaRepository = lochaRepository;

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateLocha([FromBody] LochaDto lochaCreateDto)
        {
            if(lochaCreateDto == null)
                return BadRequest(ModelState);

            var locha = _lochaRepository.GetLochy()
                .Where(l => l.NumerLochy == lochaCreateDto.NumerLochy && 
                                            (l.Status == StatusLochy.Wolna || 
                                             l.Status == StatusLochy.Pokryta || 
                                             l.Status == StatusLochy.Karmiaca))
                .FirstOrDefault();

            if(locha != null)
            {
                ModelState.AddModelError("e", "Istnieje już aktywna locha o podanym numerze.");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var lochaCreate = _lochaRepository.MapToModel(lochaCreateDto);

            var createOutput = _lochaRepository.CreateLocha(lochaCreate, lochaCreateDto.WydarzeniaLochyId);
            if(!createOutput.Item1)
            {
                ModelState.AddModelError("e", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok(createOutput.Item2);
        }
        [HttpPost("import")]
        public IActionResult ImportLochyFromFile(IFormFile file)
        {
            string uploadOutput = new UploadHandler().Upload(file);
            if(!_lochaRepository.ImportLochyFromFile(Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Lochy.csv")))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy importowaniu danych.");
                return StatusCode(500, ModelState);
            }
            return Ok(uploadOutput + " i zaimportowany pomyślnie!");
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LochaDto>))]
        public IActionResult GetLochy()
        {
            var lochy = _lochaRepository.GetLochy().ToList();
            var dtos = _lochaRepository.MapToDtoList(lochy);

             if(!ModelState.IsValid)
                return BadRequest(ModelState);
 
            return Ok(dtos);
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LochaDto>))]
        public IActionResult GetLochyByStatus(int status)
        {
            var lochy = _lochaRepository.GetLochyByStatus(status).ToList();
            var dtos = _lochaRepository.MapToDtoList(lochy);

             if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dtos);
        }

        [HttpGet("{lochaId}")]
        [ProducesResponseType(200, Type = typeof(LochaDto))]
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
        public IActionResult UpdateLocha(int lochaId, [FromBody]LochaDto updatedLochaDto)
        {
            if(updatedLochaDto == null)
                return BadRequest(ModelState);

            if(!_lochaRepository.LochaExists(lochaId))
                return NotFound();

            if(lochaId != updatedLochaDto.Id)
                return BadRequest();

            if(!ModelState.IsValid)
                return BadRequest();
            
            var updatedLocha = _lochaRepository.MapToModel(updatedLochaDto);
            
            if(!_lochaRepository.UpdateLocha(updatedLocha, updatedLochaDto.WydarzeniaLochyId))
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