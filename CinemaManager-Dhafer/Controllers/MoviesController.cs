using CinemaManager_Dhafer.Models.Cinema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaManager_Dhafer.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CinemaDbContext _context;

        public MoviesController(CinemaDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Producer)  // Include producer data
                .ToListAsync();
            return View(movies);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Producer)
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
            ViewBag.Producers = _context.Producers.ToList(); // For dropdown
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ProducerId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Producers = _context.Producers.ToList(); // Repopulate dropdown if error
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewBag.Producers = _context.Producers.ToList();
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ProducerId")] Movie movie)
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
            ViewBag.Producers = _context.Producers.ToList();
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        // Add this action to your MoviesController
        public async Task<IActionResult> MoviesAndTheirProds()
        {
            var moviesWithProducers = await _context.Movies
                .Include(m => m.Producer)  // Eager load the Producer data
                .ToListAsync();

            return View(moviesWithProducers);
        }
        public IActionResult MoviesAndTheirProds_UsingModel()
        {
            var query = from m in _context.Movies
                        join p in _context.Producers
                        on m.ProducerId equals p.Id
                        select new ProdMovie
                        {
                            mTitle = m.Title,
                            mGenre = m.Genre,
                            pName = p.Name,
                            pNat = p.Nationality
                        };

            var result = query.ToList();
            return View(result);
        }
        public async Task<IActionResult> MyMovies(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Récupère le producteur avec ses films
            var producer = await _context.Producers
                .Include(p => p.Movies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producer == null)
            {
                return NotFound();
            }

            ViewBag.ProducerName = producer.Name;
            return View(producer.Movies);
        }
        public async Task<IActionResult> SearchByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return RedirectToAction(nameof(Index));
            }

            // LINQ Query Syntax
            var movies = from m in _context.Movies
                         where m.Title.Contains(title)
                         select m;

            ViewBag.SearchTerm = title; // Pass search term to view
            return View("Index", await movies.ToListAsync()); // Reuse Index view
        }
        public async Task<IActionResult> SearchByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return RedirectToAction(nameof(Index));
            }

            // Syntaxe LINQ demandée
            var movies = from m in _context.Movies
                         where m.Genre.Contains(genre)
                         select m;

            ViewBag.SearchTerm = genre;
            ViewBag.SearchType = "genre";
            return View("Index", await movies.ToListAsync());
        }
        private List<string> GetAllGenres()
        {
            return _context.Movies
                .Select(m => m.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }
        public IActionResult SearchBy2(string title, string genre)
        {
            // Get all distinct genres for the dropdown
            ViewBag.Genres = GetAllGenres();
            ViewBag.SelectedGenre = genre;

            // Start with all movies
            var query = from m in _context.Movies
                        select m;

            // Apply title filter if provided
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(m => m.Title.Contains(title));
            }

            // Apply genre filter if provided and not "All"
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre == genre);
            }

            // Pass the search criteria to the view
            ViewBag.SearchTitle = title;

            return View(query.ToList());
        }
    }

}