using HerdRest.Dto;
using HerdRest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiotController(IMiotRepository miotRepository) : ControllerBase
    {
        private readonly IMiotRepository _miotRepository = miotRepository;

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateMiot([FromBody] MiotDto miotCreateDto)
        {
            if (miotCreateDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (miotCreateDto.WydarzeniaMiotuId == null)
            {
                ModelState.AddModelError("", "WydarzeniaMiotuId nie może być puste.");
                return BadRequest(ModelState);
            }

            var miotCreate = _miotRepository.MapToModel(miotCreateDto);
            var createOutput = _miotRepository.CreateMiot(miotCreate, miotCreateDto.WydarzeniaMiotuId.First());

            if (!createOutput.Item1)
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok(createOutput.Item2);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MiotDto>))]
        public IActionResult GetMioty()
        {
            var mioty = _miotRepository.GetMioty().ToList();
            var dtos = _miotRepository.MapToDtoList(mioty);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dtos);
        }

        [HttpGet("{miotId}")]
        [ProducesResponseType(200, Type = typeof(MiotDto))]
        [ProducesResponseType(400)]
        public IActionResult GetMiot(int miotId)
        {
            if (!_miotRepository.MiotExists(miotId))
                return NotFound();

            var miot = _miotRepository.GetMiot(miotId);
            var dto = _miotRepository.MapToDto(miot);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dto);
        }

        [HttpGet("{year}/{month}")]
        public IActionResult GetMiotyWDanymMiesiacu(int year, int month)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mioty = _miotRepository.GetMiotyWDanymMiesiacu(year, month).ToList();
            var dto = _miotRepository.MapToDtoList(mioty);            

            return Ok(dto);
        }

        [HttpPut("{miotId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateMiot(int miotId, [FromBody] MiotDto updatedMiotDto)
        {
            if (updatedMiotDto == null)
                return BadRequest(ModelState);

            if (miotId != updatedMiotDto.Id)
                return BadRequest(ModelState);

            if (!_miotRepository.MiotExists(miotId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var updatedMiot = _miotRepository.MapToModel(updatedMiotDto);
            var updateOutput = _miotRepository.UpdateMiot(updatedMiot, updatedMiotDto.WydarzeniaMiotuId);

            if (!updateOutput.Item1)
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu zmian.");
                return StatusCode(500, ModelState);
            }

            return Ok(updateOutput.Item2);
        }

        [HttpPut("weaning")]
        public IActionResult OdsadzenieWielu([FromBody] OdsadzanieDto odsadzanieDto)
        {
            if (odsadzanieDto == null)
            {
                ModelState.AddModelError("Empty", "Nie przesłano danych");
                return BadRequest(ModelState);
            }
            foreach (int miotId in odsadzanieDto.MiotyId)
            {
                if (!_miotRepository.MiotExists(miotId))
                    return NotFound("Nie odnaleziono miotu jednej z podanych loch");
            }

            if (!ModelState.IsValid)
                return BadRequest("Niepoprawne dane");

            if (!_miotRepository.AddOdsadzenia(odsadzanieDto))
                return BadRequest("Błąd przy próbie dodania odsadzeń");

            return Ok("Dodano odsadzenia pomyślnie");
        }

        [HttpDelete("{miotId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteMiot(int miotId)
        {
            if (!_miotRepository.MiotExists(miotId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var miotToDelete = _miotRepository.GetMiot(miotId);

            if (!_miotRepository.DeleteMiot(miotToDelete))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy usuwaniu");
            }

            return Ok("Usunięto pomyślnie!");
        }
    }
}