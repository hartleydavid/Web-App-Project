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

namespace Group_Project.Controllers
{
    public class ShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

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
                return RedirectToAction("Index", "Movies"); // Redirect to the home page, adjust as needed
            }
            catch (Exception ex)
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
            //Foreach value in the list
            foreach (var genre in genreList)
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

            //Convert to useable data
            dynamic ApiData = JsonConvert.DeserializeObject<dynamic>(response.Content);

            //Pull the data that we need
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

        [Authorize]
        public async Task<IActionResult> Index()
        {

            //Loop to have multiple pages?
            var options = new RestClientOptions("https://api.themoviedb.org/3/tv/top_rated?language=en-US&page=1");
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
                //If this is a new show, add it to the DB
                if (IsNewShow((string)show.title, (string)show.overview))
                {
                    //Show newShow = ;
                    // Add the new show to the context
                    _context.Show.Add(await CreateShow((int)show.id));
                }
            }


            // Save changes to the database
            await _context.SaveChangesAsync();//.ConfigureAwait(false);

            return View(await _context.Show.ToListAsync());
        }

        [Authorize]

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .FirstOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        private bool IsNewShow(string title, string description)
        {
            return !_context.Show.Any(e => e.Title.Equals(title) && e.Description.Equals(description));
        }
    }
}
