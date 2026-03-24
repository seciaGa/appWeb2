using Microsoft.AspNetCore.Mvc;
using appWeb2.Data;
using Microsoft.EntityFrameworkCore;
using appWeb2.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public async Task<IActionResult> Create(VideoJuegos juegos, IFormFile archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(juegos);

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                juegos.Imagen = "/imagenes/" + nombreArchivo;
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
        public async Task<IActionResult> Edit(int id, VideoJuegos juegos, IFormFile? archivoImagen)
        {
            if (id != juegos.Id)
                return NotFound();

            var juegoDB = await _context.VideoJuegos.FindAsync(id);

            if (juegoDB == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                juegoDB.Titulo = juegos.Titulo;
                juegoDB.Precio = juegos.Precio;
                juegoDB.Categoria = juegos.Categoria;
                juegoDB.Descripcion = juegos.Descripcion;

                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(juegoDB.Imagen))
                    {
                        var rutaAnterior = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            juegoDB.Imagen.TrimStart('/')
                        );

                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

                    var rutaNueva = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/imagenes",
                        nombreArchivo);

                    using (var stream = new FileStream(rutaNueva, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }

                    juegoDB.Imagen = "/imagenes/" + nombreArchivo;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(juegoDB);
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
