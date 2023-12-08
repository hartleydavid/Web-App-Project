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
    public class MoviesAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MoviesAPIController/5/comments
        /** Method will return all comments for a movie
         * @param int id: The ID of the movie to get comments from
         */
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<string>>> GetMovieComments(int id)
        {
            var movie = await _context.Movie
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie.Comments.Select(c => c.Text).ToList());
        }
    }
}
