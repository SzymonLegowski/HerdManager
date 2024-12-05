using HerdRest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerdRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LochaController : ControllerBase  // Użyj 'ControllerBase', nie 'Controller' w przypadku Web API
    {
        private readonly ApplicationDbContext _context;

        public LochaController(ApplicationDbContext context)
        {
            _context = context;
        }

    
        [HttpGet]
        public async Task<IActionResult> GetLochy()
        {
            var locha = await _context.Lochy.ToListAsync();
            return Ok(locha);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocha(int id)
        {
            var locha = await _context.Lochy.FindAsync(id);
            if( locha == null )
            {
                return NotFound();
            }
            return Ok(locha);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocha([FromBody] Locha newLocha)
        {
            if(newLocha == null)
            {
                return BadRequest("Invalid data");
            }
             _context.Lochy.Add(newLocha);
             await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocha), new { id = newLocha.Id }, newLocha);
        }
    }
}