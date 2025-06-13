using HerdRest.Dto;
using HerdRest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WydarzenieController(IWydarzenieRepository wydarzenieRepository) : ControllerBase
    {
        private readonly IWydarzenieRepository _wydarzenieRepository = wydarzenieRepository;

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateWydarzenie([FromBody] WydarzenieDto wydarzenieCreateDto)
        {
            if((wydarzenieCreateDto.LochyId == null && wydarzenieCreateDto.MiotyId == null) || (wydarzenieCreateDto.LochyId?.Count == 0 && wydarzenieCreateDto.MiotyId?.Count == 0))
                return BadRequest("Wydarzenie musi być przypisane do co najmniej jednej lochy lub miotu.");

            if(wydarzenieCreateDto == null)
                return BadRequest(ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var CzyIstnieje = _wydarzenieRepository.GetWydarzenia().Where(w => w.DataWydarzenia == wydarzenieCreateDto.DataWydarzenia && w.TypWydarzenia == wydarzenieCreateDto.TypWydarzenia).FirstOrDefault();

            if (CzyIstnieje != null)
            {
                ModelState.AddModelError("e", "Istnieje już krycie o podanej dacie.");
                return StatusCode(422, ModelState);
            }

            var wydarzenieCreate = _wydarzenieRepository.MapToModel(wydarzenieCreateDto);
            var createOutput =_wydarzenieRepository.CreateWydarzenie(wydarzenieCreate, wydarzenieCreateDto.MiotyId, wydarzenieCreateDto.LochyId);
            if(!createOutput.Item1)
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }

            return Ok(createOutput.Item2);
        }
       
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<WydarzenieDto>))]
        public IActionResult GetWydarzenia()
        {
            var wydarzenia = _wydarzenieRepository.GetWydarzenia().ToList();
            var dtos = _wydarzenieRepository.MapToDtoList(wydarzenia);

             if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dtos);
        }

        [HttpGet("{wydarzenieId}")]
        [ProducesResponseType(200, Type = typeof(WydarzenieDto))]
        [ProducesResponseType(400)]
        public IActionResult GetWydarzenie(int wydarzenieId)
        {
            if(!_wydarzenieRepository.WydarzenieExists(wydarzenieId))
                return NotFound();
            
            var wydarzenie = _wydarzenieRepository.GetWydarzenie(wydarzenieId);
            var dto = _wydarzenieRepository.MapToDto(wydarzenie);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dto);
        }

        [HttpGet("month/{rok}/{miesiac}")]
        public IActionResult GetWydarzeniaMiesiaca(int miesiac, int rok)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(_wydarzenieRepository.MapToDtoList([.. _wydarzenieRepository.GetWydarzeniaMiesiaca(miesiac, rok)]));
        }

        [HttpPut("{wydarzenieId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateWydarzenie(int wydarzenieId, [FromBody]WydarzenieDto updatedWydarzenieDto)
        {
             if((updatedWydarzenieDto.LochyId == null && updatedWydarzenieDto.MiotyId == null) || (updatedWydarzenieDto.LochyId?.Count == 0 && updatedWydarzenieDto.MiotyId?.Count == 0))
                return BadRequest("Wydarzenie musi być przypisane do co najmniej jednej lochy lub miotu.");

            if(updatedWydarzenieDto == null)
                return BadRequest(ModelState);

            if(wydarzenieId != updatedWydarzenieDto.Id)
                return BadRequest(ModelState);

            if(!_wydarzenieRepository.WydarzenieExists(wydarzenieId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();
            
            var updatedWydarzenie = _wydarzenieRepository.MapToModel(updatedWydarzenieDto);
            var updateOutput = _wydarzenieRepository.UpdateWydarzenie(updatedWydarzenie, updatedWydarzenieDto.MiotyId, updatedWydarzenieDto.LochyId);
            if(!updateOutput.Item1)
                {
                    ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu zmian.");
                    return StatusCode(500, ModelState);
                }

            return Ok(updateOutput.Item2);
        }
        
        [HttpDelete("{wydarzenieId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteWydarzenie(int wydarzenieId)
        {
            if(!_wydarzenieRepository.WydarzenieExists(wydarzenieId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var wydarzenieToDelete = _wydarzenieRepository.GetWydarzenie(wydarzenieId);

            if(!_wydarzenieRepository.DeleteWydarzenie(wydarzenieToDelete))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy usuwaniu");
            }

            return Ok("Usunięto pomyślnie!");
        }
    }
}