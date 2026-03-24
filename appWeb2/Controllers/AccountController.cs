using Microsoft.AspNetCore.Mvc;
using appWeb2.Data;
using System.Diagnostics;
using appWeb2.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography;



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
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            //var user = _context.Usuarios
             //   .FirstOrDefault(u => u.Email == model.correo && u.Password == model.password);

            //if (user != null)
            //{
              //  HttpContext.Session.SetString("usuario", user.Nombre);
              //  Console.WriteLine("Usuario logueado: " + user.Nombre);
               // return RedirectToAction("Index", "Home");
            //}

            //ViewBag.Error = "Correo o contraseña incorrectos";
            //return View();

            var user = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.correo);

            if (user != null)
            { 
                string saltedPassword = user.salt + model.password;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    if (hashBytes.SequenceEqual(user.Password))
                    {
                        HttpContext.Session.SetString("Usuario", user.Nombre);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.Error = "Credenciales Incorrectas";
            return View();
        }
    }
}
