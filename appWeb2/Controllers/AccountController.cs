using Microsoft.AspNetCore.Mvc;
using appWeb2.Data;
using System.Diagnostics;
using appWeb2.Models;
using Microsoft.AspNetCore.Http;



namespace appWeb2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.correo && u.Password == model.password);

            if (user != null)
            {
                HttpContext.Session.SetString("usuario", user.Nombre);
                Console.WriteLine("Usuario logueado: " + user.Nombre);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }
    }
}
