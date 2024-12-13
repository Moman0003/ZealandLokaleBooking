using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class ManageRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }
        public string Filter { get; set; } // Filtrering af lokaler

        public void OnGet(string filter = "all")
        {
            Filter = filter;

            if (filter == "booked")
            {
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .Include(r => r.Bookings.Where(b => b.IsActive))
                    .Where(r => r.Bookings.Any(b => b.IsActive && b.EndTime >= DateTime.Now))
                    .ToList();
            }
            else if (filter == "available")
            {
                Rooms = _context.Rooms
                    .Include(r => r.Bookings)
                    .Where(r => !r.Bookings.Any(b => b.IsActive && b.EndTime >= DateTime.Now))
                    .ToList();
            }
            else // "all"
            {
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .Include(r => r.Bookings.Where(b => b.IsActive))
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int roomId)
        {
            var room = _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefault(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["ErrorMessage"] = "Lokalet findes ikke.";
                return RedirectToPage(new { filter = Filter });
            }

            // Find den første aktive booking og annuller den
            var booking = room.Bookings.FirstOrDefault(b => b.IsActive && b.EndTime >= DateTime.Now);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.IsActive = false;

                if (!room.Bookings.Any(b => b.IsActive))
                {
                    room.IsBooked = false;
                    room.BookedByUserId = null;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Bookingen for lokalet \"{room.RoomName}\" blev annulleret.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ingen aktive bookinger blev fundet.";
            }

            return RedirectToPage(new { filter = Filter });
        }
    }
}













