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
            };

            return newMovie;
        }

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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Genre,ReleaseDate,BoxOfficeTotal,IMBDScore")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Genre,ReleaseDate,BoxOfficeTotal,IMBDScore")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
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

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }

        private bool IsNewMovie(string title, string description)
        {
            return !_context.Movie.Any(e => e.Title.Equals(title) && e.Description.Equals(description));
        }
    }
}
