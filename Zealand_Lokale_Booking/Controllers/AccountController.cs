using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZealandLokaleBooking.Data; // Namespace til ApplicationDbContext
using ZealandLokaleBooking.Models; // Namespace til User modellen
using System.Linq;
using Microsoft.EntityFrameworkCore; // For LINQ

namespace ZealandLokaleBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Returnerer Login.cshtml i Views/Account
        }

        // POST: /Account/Login
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Find brugeren i databasen
            var user = _context.Users.Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            Console.WriteLine($"Login forsøg: Email = {email}, Rolle = {user?.Role?.RoleName}");

            if (user != null)
            {
                // Opret brugerens claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email), // Email som Name
                    new Claim("role", user.Role.RoleName)  // Rollen
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Log brugeren ind
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Debugger redirect-logik
                if (user.Role.RoleName == "Student")
                {
                    Console.WriteLine("Elev logget ind, omdirigerer til StudentDashboard.");
                    return RedirectToPage("/StudentDashboard");
                }
                else if (user.Role.RoleName == "Teacher")
                {
                    Console.WriteLine("Lærer logget ind, omdirigerer til TeacherDashboard.");
                    return RedirectToPage("/TeacherDashboard");
                }
            }

            // Hvis login fejler
            Console.WriteLine("Login mislykkedes. Ugyldig email eller adgangskode.");
            ViewBag.ErrorMessage = "Ugyldig email eller adgangskode.";
            return View();
        }

        


        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index"); // Redirect korrekt til Razor Page Index
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Returnerer Register.cshtml i Views/Account
        }
        

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, string phoneNumber)
        {
            // Bestem rolle baseret på email
            int roleId = email.Contains("edu.zealand.dk") ? 1 : 2; // 1 = Student, 2 = Teacher

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                RoleId = roleId
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Din konto er blevet oprettet. Du kan nu logge ind.";
            return RedirectToPage("/Index");
        }



    }
}