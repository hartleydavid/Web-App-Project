using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group_Project.Data;
using Group_Project.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RestSharp;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace Group_Project.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        /** Method will search through the genre array from the API
         * and parse the data into a string listing each genre.
         * Removes the trailing comma on method return
         * 
         * @param dynamic genreList: The list of genres from the API
         * @returns: A string of all the genres from the list.
         *          Example: Drama, Horror, Action
         */
        public string GetGenresString (dynamic genreList)
        {
            string myGenres = "";
            //Foreach value in the list
            foreach(var genre in genreList)
            {
                //Add the value to the string
                myGenres += genre.name + ", ";
            }
            //Return genres
            return myGenres[0..^2];
        }

        /**
         * 
         * 
         * */
        public async Task<Movie> CreateMovie(int movieID) 
        {
            //Pull top level data from the API on the given movie id
            string apiPath = "https://api.themoviedb.org/3/movie/" + movieID + "?language=en-US";
            Console.Write("Entering string {0}", apiPath);
            var options = new RestClientOptions(apiPath);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs");
            var response = await client.GetAsync(request);

            //Convert to useable data
            dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

            //Pull the data that we need
            var newMovie = new Movie
            {
                Title = ApiData.title,
                Description = ApiData.overview,
                Genre = GetGenresString(ApiData.genres),
                ReleaseDate = DateTime.ParseExact((string)ApiData.release_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                IMBDScore = ApiData.vote_average,
                BoxOfficeTotal = ApiData.revenue,
                ImageSrc = "https://image.tmdb.org/t/p/original" + ApiData.poster_path,
                Comments = new List<Comment>(),
            };

            return newMovie;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
           
            //Loop to have multiple pages?
            var options = new RestClientOptions("https://api.themoviedb.org/3/movie/top_rated?language=en-US&page=1");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs");
            var response = await client.GetAsync(request);


            // Deserialize the API response to C# objects
            dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // Process and save data to the database
            foreach (var movie in ApiData.results)
            {
                //If this is a new movie, add it to the DB
                if (IsNewMovie((string)movie.title, (string)movie.overview))
                {
                    Movie newMovie = await CreateMovie((int)movie.id);
                    // Add the new movie to the context
                    _context.Movie.Add(newMovie);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();//.ConfigureAwait(false);

            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        [Authorize]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Details/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            //Get the movie
            var movie = await _context.Movie
               .Include(m => m.Comments)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var newComment = new Comment
            {
                MediaID = id,
                Text = comment
            };

            // Ensure that the Comments collection is initialized
            if (movie.Comments == null)
            {
                movie.Comments = new List<Comment>();
            }

            movie.Comments.Add(newComment);

            _context.Comment.Add(newComment);
            _context.SaveChanges();

            return View("Details", movie);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveComment(int id)
        {
            // Find the comment by its ID
            var comment = await _context.Comment.FindAsync(id);

            //Get the movie
            var movie = await _context.Movie
               .Include(m => m.Comments)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            movie.Comments.Remove(comment);

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return View("Details", movie);
        }

        public async Task<IActionResult> RemoveAllRecords()
        {
            try
            {
                // Get all records from the database
                var allRecords = await _context.Movie.ToListAsync();

                // Remove each record
                _context.Movie.RemoveRange(allRecords);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Optionally, redirect to another action or return a success message
                return RedirectToAction("Index", "Movies"); // Redirect to the home page, adjust as needed
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, show an error message, etc.)
                return RedirectToAction("Error", "Home");
            }
        }

        private bool IsNewMovie(string title, string description)
        {
            return !_context.Movie.Any(e => e.Title.Equals(title) && e.Description.Equals(description));
        }
    }
}
