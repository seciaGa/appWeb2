using Microsoft.AspNetCore.Mvc;
using appWeb2.Data;
using System.Diagnostics;
using appWeb2.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using appWeb2.Filtros;




namespace appWeb2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            var data = (from v in _context.VideoJuegos
                        join c in _context.Categorias
                        on v.CategoriaId equals c.Id 
                        group v by c.Nombre into g
                        select new
                        {
                            Categoria = g.Key,
                            Total = g.Count()
                        }).ToList();
            ViewBag.Categorias = data.Select(x => x.Categoria).ToList();
            ViewBag.Totales = data.Select(x => x.Total).ToList();
            return View();
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

            Console.WriteLine("Correo ingresado: " + model.correo);

            if (user != null)
            {
                Console.WriteLine("Usuario encontrado: " + user.Nombre);

                string saltedPassword;

                if (user.salt != null)
                {
                    saltedPassword = user.salt.ToString() + model.password;
                    Console.WriteLine("Usando SALT: " + user.salt.ToString());
                }
                else
                {
                    saltedPassword = model.password;
                    Console.WriteLine("SIN salt");
                }

                Console.WriteLine("Password ingresado: " + model.password);
                Console.WriteLine("Password combinado: " + saltedPassword);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    Console.WriteLine("Hash generado: " + BitConverter.ToString(hashBytes));
                    Console.WriteLine("Hash BD: " + BitConverter.ToString(user.Password));

                    if (hashBytes.SequenceEqual(user.Password))
                    {
                        Console.WriteLine("LOGIN CORRECTO ✅");

                        HttpContext.Session.SetString("Usuario", user.Nombre);
                        return RedirectToAction("Dashboard", "Account");
                    }
                    else
                    {
                        Console.WriteLine("HASH NO COINCIDE ❌");
                    }
                }
            }
            else
            {
                Console.WriteLine("Usuario NO encontrado ❌");
            }

            ViewBag.Error = "Credenciales Incorrectas";
            return View();
        }
    }
}
