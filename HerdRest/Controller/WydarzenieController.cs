using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerdRest.Dto;
using HerdRest.Interfaces;
using HerdRest.Model;
using Microsoft.AspNetCore.Mvc;

namespace HerdRest.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WydarzenieController : ControllerBase
    {
        private readonly IWydarzenieRepository _wydarzenieRepository;
        private readonly ILochaRepository _lochaRepository;
        public WydarzenieController(IWydarzenieRepository wydarzenieRepository, ILochaRepository lochaRepository)
        {
            _wydarzenieRepository = wydarzenieRepository;
            _lochaRepository = lochaRepository;
        }
         [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateWydarzenie([FromBody] Wydarzenie wydarzenieCreate, int? miotId, int? lochaId)
        {
            if(wydarzenieCreate == null)
                return BadRequest(ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_wydarzenieRepository.CreateWydarzenie(wydarzenieCreate, miotId, lochaId))
            {
                ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu.");
                return StatusCode(500, ModelState);
            }


            return Ok("Dodano pomyślnie!");
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<WydarzenieDto>))]
        public IActionResult GetLochy()
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

        [HttpPut("{wydarzenieId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateWydarzenie(int wydarzenieId, [FromBody]Wydarzenie updatedWydarzenie)
        {
            if(updatedWydarzenie == null)
                return BadRequest(ModelState);

            if(wydarzenieId != updatedWydarzenie.Id)
                return BadRequest(ModelState);

            if(!_wydarzenieRepository.WydarzenieExists(wydarzenieId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();
            
            if(!_wydarzenieRepository.UpdateWydarzenie(updatedWydarzenie))
                {
                    ModelState.AddModelError("", "Coś poszło nie tak przy zapisywaniu zmian.");
                    return StatusCode(500, ModelState);
                }

            return Ok("Zapisano pomyślnie!");
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