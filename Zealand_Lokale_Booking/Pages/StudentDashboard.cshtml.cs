using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Zealand_Lokale_Booking.Pages
{
    public class StudentDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ZealandLokaleBooking.Models.Booking> Bookings { get; set; }

        public void OnGet()
        {
            var userEmail = User.Identity.Name;

            // Hent bookingerne for den aktuelle bruger, inklusive StartTime og EndTime
            Bookings = _context.Bookings
                .Where(b => b.User.Email == userEmail) // Filter bookingerne baseret på brugerens email
                .Include(b => b.Room) // Inkluder relaterede room data
                .Include(b => b.User) // Inkluder brugerens data
                .ToList(); // Hent resultaterne som en liste
        }
    }
}