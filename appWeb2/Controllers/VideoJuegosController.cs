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

        // 🔹 LISTAR JUEGOS
        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos
                .Include(x => x.Categoria)
                .ToListAsync();

            return View(juegos);
        }

        // 🔹 CREATE (GET)
        public IActionResult Create()
        {
            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Categorias,
                "Id",
                "Nombre"
            );

            return View();
        }

        // 🔹 CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoJuegos juegos, IFormFile archivoImagen)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    _context.Categorias,
                    "Id",
                    "Nombre",
                    juegos.CategoriaId
                );

                return View(juegos);
            }

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                juegos.Imagen = "/imagenes/" + nombreArchivo;
            }

            _context.Add(juegos);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 🔹 EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego == null) return NotFound();

            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Categorias,
                "Id",
                "Nombre",
                juego.CategoriaId
            );

            return View(juego);
        }

        // 🔹 EDIT (POST)
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
                juegoDB.CategoriaId = juegos.CategoriaId;
                juegoDB.Descripcion = juegos.Descripcion;
                juegoDB.EdadMinima = juegos.EdadMinima;
                juegoDB.EnPromocion = juegos.EnPromocion;

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

            ViewBag.Categorias = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Categorias,
                "Id",
                "Nombre",
                juegos.CategoriaId
            );

            return View(juegoDB);
        }

        // 🔹 DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.VideoJuegos
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }

        // 🔹 DELETE (POST)
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

        // 🔹 LISTAR CATEGORÍAS
        public IActionResult Categorias()
        {
            var categorias = _context.Categorias.ToList();
            return View(categorias);
        }

        // 🔹 JUEGOS POR CATEGORÍA
        public IActionResult PorCategoria(int id)
        {
            var juegos = _context.VideoJuegos
                .Include(x => x.Categoria)
                .Where(x => x.CategoriaId == id)
                .ToList();

            return View("Nuevos", juegos);
        }

        // 🔹 NUEVOS
        public IActionResult Nuevos()
        {
            var juegos = _context.VideoJuegos
                .Include(x => x.Categoria)
                .OrderByDescending(x => x.FechaRegistro)
                .Take(15)
                .ToList();

            return View(juegos);
        }

        // 🔹 PROMOCIONES
        public IActionResult Promociones()
        {
            var juegos = _context.VideoJuegos
                .Include(x => x.Categoria)
                .Where(x => x.EnPromocion == true)
                .ToList();

            return View("Index", juegos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var juego = await _context.VideoJuegos
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (juego == null)
                return NotFound();

            return View(juego);
        }
    }
}