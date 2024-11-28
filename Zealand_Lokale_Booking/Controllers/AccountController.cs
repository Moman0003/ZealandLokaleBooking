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
        public async Task<IActionResult> Login(string email, string password)
        {
            // Find brugeren i databasen
            var user = _context.Users.Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            Console.WriteLine($"User: {user?.Name}, Role: {user?.Role?.RoleName}");
            
            if (user != null)
            {
                // Opret brugerens claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("role", user.Role.RoleName) // Brug rollen fra databasen
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Log brugeren ind
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect baseret på rollen
                if (user.Role.RoleName == "Student")
                {
                    return RedirectToPage("/StudentDashboard"); // Razor Page path
                }
                else if (user.Role.RoleName == "Teacher")
                {
                    return RedirectToAction("ManageRooms", "Teacher"); // Controller/Action path
                }
            }

            // Hvis login fejler
            ViewBag.ErrorMessage = "Ugyldig email eller adgangskode.";
            return View(); // Forbliv på login-siden
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
        public IActionResult Register(string name, string email, string password)
        {
            // Opret en ny bruger
            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = password,
                RoleId = 1 // Standard "Student" rolle-id
            };

            // Tilføj brugeren til databasen
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Send en bekræftelsesmeddelelse til forsiden
            TempData["SuccessMessage"] = "Din konto er blevet oprettet som Elev. Du kan nu logge ind.";

            // Redirect til forsiden
            return RedirectToPage("/Index");
        }

    }
}