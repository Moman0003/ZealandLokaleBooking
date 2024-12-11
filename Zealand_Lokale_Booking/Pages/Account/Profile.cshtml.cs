using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using System;

using System.Linq; // Tilføj dette for at få adgang til ToList() og LINQ

namespace Zealand_Lokale_Booking.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User CurrentUser { get; set; } // Omdøbt for at sikre klarhed

        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Debugging: Udskriv værdien af User.Identity.Name
                Console.WriteLine($"User.Identity.Name: {User.Identity?.Name}");

                var email = User.Identity.Name; // Hent email fra Identity
                CurrentUser = _context.Users
                    .Include(u => u.Role) // Inkluder relateret rolle
                    .FirstOrDefault(u => u.Email == email);

                if (CurrentUser == null)
                {
                    Console.WriteLine("Brugeren kunne ikke findes i databasen.");
                }
                else
                {
                    Console.WriteLine($"Bruger fundet: {CurrentUser.FirstName} {CurrentUser.LastName}, {CurrentUser.Email}");
                }
            }
            else
            {
                Console.WriteLine("Bruger er ikke logget ind.");
            }
        }
    }
}
