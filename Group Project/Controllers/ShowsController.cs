using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group_Project.Data;
using Group_Project.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Group_Project.Controllers
{
    public class ShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Used in testing
        public async Task<IActionResult> RemoveAllRecords()
        {
            try
            {
                // Get all records from the database
                var allRecords = await _context.Show.ToListAsync();

                // Remove each record
                _context.Show.RemoveRange(allRecords);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Optionally, redirect to another action or return a success message
                return RedirectToAction("Index", "Shows"); // Redirect to the home page, adjust as needed
            }
            catch (Exception)
            {
                // Handle the exception (log it, show an error message, etc.)
                return RedirectToAction("Error", "Home");
            }
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

        /** Creates a show object that is the showID's value in the TMDB API
         * Will pull all the needed data for the given show and create the show object 
         * and return it.
         * @param int showID: The ID of the show we want to get top level details of from the API
         * @Return The new show of the showID
         */
        public async Task<Show> CreateShow(int showID)
        {
            //Pull top level data from the API on the given show id
            string apiPath = "https://api.themoviedb.org/3/tv/" + showID + "?language=en-US";
            var options = new RestClientOptions(apiPath);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs");
            var response = await client.GetAsync(request);

            // Convert to useable data
            dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // Pull the data that we need
            var newShow = new Show
            {
                Title = ApiData.name,
                Description = ApiData.overview,
                Genre = GetGenresString(ApiData.genres),
                ReleaseDate = DateTime.ParseExact((string)ApiData.first_air_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                LastAirDate = DateTime.ParseExact((string)ApiData.last_air_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                IMBDScore = ApiData.vote_average,
                Seasons = ApiData.number_of_seasons,
                Episodes = ApiData.number_of_episodes,
                ImageSrc = "https://image.tmdb.org/t/p/original" + ApiData.poster_path,
                Comments = new List<Comment>(),
            };

            return newShow;
        }

        /** Method will pull shows from the TMBD API and populate our database when the user opens the index view
         * Function gets called when the index view is called for in the website.
         * Requires users to be logged in to view the data
         */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // The string link to the API that we will pull data from, missing page #
            string apiLink = "https://api.themoviedb.org/3/tv/top_rated?language=en-US&page=";
            // The number of pages we will pull from
            int pageCount = 3;

            // Loop for the number of pages we want to access from the API
            for (int i = 1; i <= pageCount; i++)
            {
                // Concate the page number to the link
                var options = new RestClientOptions(apiLink + i);
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MGVjNDc0YTJhZjVhNjMzZTUxOWM1NWY4NGYxYTAxMCIsInN1YiI6IjY1NWQ0OWZmZmFiM2ZhMDBmZWNjZjk4NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.EvJfo5-I1AS2ro4I8mWfrzSHKUEuHQJQR_KolK-WSHs");
                var response = await client.GetAsync(request);

                // Deserialize the API response to C# objects
                dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Process and save data to the database
                foreach (var show in ApiData.results)
                {
                    // If this is a new show, add it to the DB
                    if (IsNewShow((string)show.name, (string)show.overview))
                    {
                        Show newShow = await CreateShow((int)show.id);
                        // Add the new show to the context
                        _context.Show.Add(newShow);
                    }
                }
            }
            // Save changes to the database
            await _context.SaveChangesAsync();
            
            // Return view of all shows
            return View(await _context.Show.ToListAsync());
        }
        /** Template Details View from scaffolded objects
         * Added .Include() function call for the context of our model
         * Requires users to be logged in to view the data
         */
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .Include(m => m.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        /** Method will add a comment to the respective show that the user is currently on
         * @param int id: The ID of the show that we will add the comment to
         * @param string comment: The comment that was added to the show
         */
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            // Get the show
            var show = await _context.Show
               .Include(m => m.Comments)
               .FirstOrDefaultAsync(m => m.Id == id);

            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Assert != null
            if (show == null)
            {
                return NotFound();
            }

            // Create a new comment object 
            var newComment = new Comment
            {
                MediaID = id,
                Text = comment,
                AuthorId = authorId,
                Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId),
                DatePosted = DateTime.Now
            };

            // Ensure that the Comments collection is initialized
            if (show.Comments == null)
            {
                show.Comments = new List<Comment>();
            }

            // Add the comments
            show.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            // Return the updated view
            return RedirectToAction("Details", new { id = show.Id });
        }

        /** Method will remove a comment to the respective show that the user is currently on
         * @param int id: The ID of the comment we are going to remove
         */
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveComment(int commentId)
        {

            // Find the comment by its ID
            var comment = await _context.Comment.FindAsync(commentId);

            // Assert not null
            if (comment == null)
            {
                return NotFound();
            }

            // Get the show by the mediaID from the comment
            var show = await _context.Show
               .Include(m => m.Comments)
               .FirstOrDefaultAsync(m => m.Id == comment.MediaID);

            // Assert not null
            if (show == null)
            {
                return NotFound();
            }

            // Remove comments
            show.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            // Return updated view
            return RedirectToAction("Details", new { id = show.Id });
        }

        /** Method will test if the given show is a new show in our database
         * Does this by seeing if the title and desctiption are already in the database
         * @Param string title: The title of the show
         * @Param string description: The description of the show
         * @Returns True if the show is new
         *          False if the show is already in the DB
         **/
        private bool IsNewShow(string title, string description)
        {
            return !_context.Show.Any(e => e.Title.Equals(title) && e.Description.Equals(description));
        }
    }
}
