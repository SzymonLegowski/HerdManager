using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiotController : ControllerBase
    {
        private readonly IMiotRepository _miotRepository;

        public MiotController(IMiotRepository miotRepository)
        {
            _miotRepository = miotRepository;
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateMiot([FromBody] Miot miotCreate)
        {
            if(miotCreate == null)
                return BadRequest(ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_miotRepository.CreateMiot(miotCreate))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok("Dodano pomyślnie!");
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Miot>))]
        public IActionResult GetMioty()
        {
            var lochy = _miotRepository.GetMioty().ToList();
            var dtos = _miotRepository.MapToDtoList(lochy);

             if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dtos);
        }

        [HttpGet("{miotId}")]
        [ProducesResponseType(200, Type = typeof(Miot))]
        [ProducesResponseType(400)]
        public IActionResult GetMiot(int miotId)
        {
            if(!_miotRepository.MiotExists(miotId))
                return NotFound();
            
            var miot = _miotRepository.GetMiot(miotId);
            var dto = _miotRepository.MapToDto(miot);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(dto);
        }

        [HttpPut("{miotId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateMiot(int miotId, [FromBody]Miot updatedMiot)
        {
            if(updatedMiot == null)
                return BadRequest(ModelState);

            if(miotId != updatedMiot.Id)
                return BadRequest(ModelState);

            if(!_miotRepository.MiotExists(miotId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();
            
            if(!_miotRepository.UpdateMiot(updatedMiot))
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
            if(!_miotRepository.MiotExists(miotId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var miotToDelete = _miotRepository.GetMiot(miotId);

            if(!_miotRepository.DeleteMiot(miotToDelete))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy usuwaniu");
            }

            return Ok("Usunięto pomyślnie!");
        }
    }
}