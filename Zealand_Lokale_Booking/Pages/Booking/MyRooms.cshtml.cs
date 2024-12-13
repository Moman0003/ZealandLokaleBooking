using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class MyRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ZealandLokaleBooking.Models.Booking> ActiveBookings { get; set; }

        public void OnGet()
        {
            var userEmail = User.Identity.Name;

            // Find den nuværende bruger
            var currentUserId = _context.Users
                .FirstOrDefault(u => u.Email == userEmail)?.UserId;

            if (currentUserId.HasValue)
            {
                // Hent kun aktive bookinger for den aktuelle bruger
                ActiveBookings = _context.Bookings
                    .Where(b => b.UserId == currentUserId.Value && b.IsActive)
                    .Include(b => b.Room) // Medtag lokaleoplysninger
                    .ToList();
            }
            else
            {
                ActiveBookings = new List<ZealandLokaleBooking.Models.Booking>(); // Ingen aktive bookinger, hvis brugeren ikke findes
            }
        }

        public IActionResult OnPostDeleteRoom(int roomId)
        {
            var booking = _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefault(b => b.RoomId == roomId && b.IsActive);

            if (booking != null)
            {
                // Markér booking som annulleret
                booking.IsActive = false;
                booking.Status = "Cancelled";
                booking.CancelledDate = DateTime.Now;

                // Opdater lokalet for at markere det som ledigt
                var room = booking.Room;
                room.IsBooked = false;
                room.BookedByUserId = null;

                _context.SaveChanges();

                // Tilføj succesmeddelelse
                TempData["SuccessMessage"] = $"Bookingen for {room.RoomName} blev annulleret.";
            }
            else
            {
                TempData["ErrorMessage"] = "Der opstod en fejl under annulleringen.";
            }

            return RedirectToPage(); // Reload siden efter sletning
        }
    }
}
