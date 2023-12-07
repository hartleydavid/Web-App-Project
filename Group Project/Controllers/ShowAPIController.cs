using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group_Project.Data;
using Group_Project.Models;

namespace Group_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ShowAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Show>>> GetShow()
        {
            return await _context.Show.ToListAsync();
        }

        // GET: api/ShowAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Show>> GetShow(int id)
        {
            var show = await _context.Show.FindAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            return show;
        }

        // PUT: api/ShowAPI/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShow(int id, Show show)
        {
            if (id != show.Id)
            {
                return BadRequest();
            }

            _context.Entry(show).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ShowAPI
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Show>> PostShow(Show show)
        {
            _context.Show.Add(show);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShow", new { id = show.Id }, show);
        }

        // DELETE: api/ShowAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Show>> DeleteShow(int id)
        {
            var show = await _context.Show.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            _context.Show.Remove(show);
            await _context.SaveChangesAsync();

            return show;
        }

        private bool ShowExists(int id)
        {
            return _context.Show.Any(e => e.Id == id);
        }
    }
}
