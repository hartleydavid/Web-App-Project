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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
        public string GetGenresString(dynamic genreList)
        {
            string myGenres = "";
            // Foreach value in the list
            foreach (var genre in genreList)
            {
                // Add the value to the string
                myGenres += genre.name + ", ";
            }
            // Return genres
            return myGenres[0..^2];
        }

        /** Creates a movie object that is the movieID's value in the TMDB API
         * Will pull all the needed data for the given movie and create the movie object 
         * and return it.
         * @param int MovieID: The ID of the movie we want to get top level details of from the API
         * @Return The new movie of the movieID
         */
        public async Task<Movie> CreateMovie(int movieID)
        {
            //Pull top level data from the API on the given movie id
            string apiPath = "https://api.themoviedb.org/3/movie/" + movieID + "?language=en-US";
            var options = new RestClientOptions(apiPath);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs");
            var response = await client.GetAsync(request);

            // Convert to useable data
            dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // Pull the data that we need
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
            // Return the new movie
            return newMovie;
        }

        /** Method will pull movies from the TMBD API and populate our database when the user opens the index view
         * Function gets called when the index view is called for in the website.
         * Requires users to be logged in to view the data
         */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // The string link to the API that we will pull data from, missing page #
            string apiLink = "https://api.themoviedb.org/3/movie/top_rated?language=en-US&page=";
            // The number of pages we will pull from
            int pageCount = 2;

            // Loop for the number of pages we want to access from the API
            for (int i = 1; i <= pageCount; i++)
            {
                // Pull the top rated movies list from the API
                var options = new RestClientOptions(apiLink + i);
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
            }
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the the view of the movies index
            return View(await _context.Movie.ToListAsync());
        }

        /** Template Details View from scaffolded objects
         * Added .Include() function call for the context of our model
         * Requires users to be logged in to view the data
         */
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            // If the ID is not correctly passed
            if (id == null)
            {
                return NotFound();
            }

            // Get the movie with the parameter ID value
            var movie = await _context.Movie
                .Include(m => m.Comments)
                .ThenInclude(comment => comment.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            // Make sure it is not null
            if (movie == null)
            {
                return NotFound();
            }

            // Return the view of the movies details
            return View(movie);
        }

        /** Method will add a comment to the respective movie that the user is currently on
         * @param int id: The ID of the movie that we will add the comment to
         * @param string comment: The comment that was added to the movie
         */
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            //Get the movie
            var movie = await _context.Movie
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Assert != null
            if (movie == null)
            {
                return NotFound();
            }

            var newComment = new Comment
            {
                MediaID = id,
                Text = comment,
                AuthorId = authorId,
                Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId),
                DatePosted = DateTime.Now
            };

            // Ensure that the Comments collection is initialized
            if (movie.Comments == null)
            {
                movie.Comments = new List<Comment>();
            }

            // Add the comments
            movie.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            // Return the updated view
            return RedirectToAction("Details", new { id = movie.Id });
        }

        /** Method will remove a comment to the respective movie that the user is currently on
         * @param int id: The ID of the comment we are going to remove
         */
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveComment(int commentId)
        {

            // Find the comment by its ID
            var comment = await _context.Comment.FindAsync(commentId);

            //Assert not null
            if (comment == null)
            {
                return NotFound();
            }

            // Get the movie by the mediaID from the comment
            var movie = await _context.Movie
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == comment.MediaID);

            // Assert not null
            if (movie == null)
            {
                return NotFound();
            }

            // Remove comments
            movie.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            // Return updated view
            return RedirectToAction("Details", new { id = movie.Id });
        }

        // Testing method that will wipe all the movies from the database
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
            catch (Exception)
            {
                // Handle the exception (log it, show an error message, etc.)
                return RedirectToAction("Error", "Home");
            }
        }

        /** Method will test if the given movie is a new movie in our database
         * Does this by seeing if the title and desctiption are already in the database
         * @Param string title: The title of the movie
         * @Param string description: The description of the movie
         * @Returns True if the movie is new
         *          False if the movie is already in the DB
         **/
        private bool IsNewMovie(string title, string description)
        {
            return !_context.Movie.Any(e => e.Title.Equals(title) && e.Description.Equals(description));
        }
    }
}
