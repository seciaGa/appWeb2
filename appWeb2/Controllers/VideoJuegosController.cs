using Microsoft.AspNetCore.Mvc;
using appWeb2.Data;
using Microsoft.EntityFrameworkCore;
using appWeb2.Models;
namespace appWeb2.Controllers
{
    public class VideoJuegosController : Controller
    {
        private readonly AppDbContext _context;

        public VideoJuegosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos.ToListAsync();
            return View(juegos);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoJuegos juegos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(juegos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(juegos);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego == null) return NotFound();

            return View(juego);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoJuegos juegos)
        {
            if (id != juegos.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(juegos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.VideoJuegos.Any(e => e.Id == juegos.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(juegos);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos.FirstOrDefaultAsync(n => n.Id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego != null)
            {
                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
