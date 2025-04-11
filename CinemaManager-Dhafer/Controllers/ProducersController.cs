using CinemaManager_Dhafer.Models.Cinema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CinemaManager_Dhafer.Controllers
{
    public class ProducersController : Controller
    {
        CinemaDbContext _context;

        public ProducersController(CinemaDbContext context)
        {
            _context = context;
        }

        // GET: Producers
        public IActionResult Index()
        {

            var producers = _context.Producers.ToList();
            return View(producers);
        }

        // GET: Producers/Details/5
        public IActionResult Details(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // GET: Producers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producer producer)
        {
            if (ModelState.IsValid)
            {
                _context.Producers.Add(producer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        // GET: Producers/Edit/5
        public IActionResult Edit(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // POST: Producers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producer producer)
        {
            if (id != producer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Producers.Update(producer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        // GET: Producers/Delete/5
        public IActionResult Delete(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        // POST: Producers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var producer = _context.Producers.Find(id);
            if (producer == null) return NotFound();

            _context.Producers.Remove(producer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        // Add this action to your ProducersController
        public async Task<IActionResult> ProdsAndTheirMovies()
        {
            var producersWithMovies = await _context.Producers
                .Include(p => p.Movies)  // Eager load the Movies data
                .ToListAsync();

            return View(producersWithMovies);
        }
        public IActionResult ProdsAndTheirMovies_UsingModel()
        {
            var query = from p in _context.Producers
                        join m in _context.Movies
                        on p.Id equals m.ProducerId into producerMovies
                        from pm in producerMovies.DefaultIfEmpty()
                        select new ProdMovie
                        {
                            pName = p.Name,
                            pNat = p.Nationality,
                            mTitle = pm != null ? pm.Title : "No movies",
                            mGenre = pm != null ? pm.Genre : "N/A"
                        };

            return View(query.ToList());
        }
    }
}
