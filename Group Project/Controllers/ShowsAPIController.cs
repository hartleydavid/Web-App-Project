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
    public class ShowsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ShowsAPIController/5/comments
        /** Method will return all comments for a show
         * @param int id: The ID of the show to get comments from
         */
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<string>>> GetMovieComments(int id)
        {
            var show = await _context.Show
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            return Ok(show.Comments.Select(c => c.Text).ToList());
        }
    }
}
