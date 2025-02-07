using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.PublicClasses;
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

            if (!_miotRepository.CreateMiot(miotCreate, miotCreateDto.WydarzeniaMiotuId.First()))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok("Dodano pomyślnie!");
        }

        [HttpPost("import")]
        public IActionResult ImportMiotyFromFile(IFormFile file)
        {
            string uploadOutput = new UploadHandler().Upload(file);
            if(!_miotRepository.ImportMiotyFromFile(Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Mioty.csv")))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy importowaniu danych.");
                return StatusCode(500, ModelState);
            }
            return Ok(uploadOutput + " i zaimportowany pomyślnie!");
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

            if (!_miotRepository.UpdateMiot(updatedMiot, updatedMiotDto.WydarzeniaMiotuId))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu zmian.");
                return StatusCode(500, ModelState);
            }

            return Ok("Zapisano pomyślnie!");
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