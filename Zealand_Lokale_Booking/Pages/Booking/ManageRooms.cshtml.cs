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

            // Hent lokaler og associerede bookinger samt brugere
            var query = _context.Rooms
                .Include(r => r.Bookings.Where(b => b.IsActive && b.Status == "Active"))
                .ThenInclude(b => b.User)
                .AsQueryable();

            if (filter == "booked")
            {
                // Vis kun lokaler med aktive bookinger
                query = query.Where(r => r.Bookings.Any(b => b.IsActive && b.Status == "Active"));
            }
            else if (filter == "available")
            {
                // Vis kun lokaler uden aktive bookinger
                query = query.Where(r => !r.Bookings.Any(b => b.IsActive && b.Status == "Active"));
            }

            Rooms = query.ToList();
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int roomId)
        {
            var room = _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefault(r => r.RoomId == roomId);

            if (room == null || !room.Bookings.Any(b => b.IsActive && b.Status == "Active"))
            {
                TempData["ErrorMessage"] = "Lokalet har ingen aktiv booking.";
                return RedirectToPage(new { filter = Filter });
            }

            // Find aktive bookinger
            var booking = room.Bookings.FirstOrDefault(b => b.IsActive && b.Status == "Active");
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.IsActive = false;
            }

            // Opdater lokalet som ledigt, hvis ingen aktive bookinger er tilbage
            if (!room.Bookings.Any(b => b.IsActive && b.Status == "Active"))
            {
                room.IsBooked = false;
                room.BookedByUserId = null;
            }

            // Opret notifikation
            var bookedUserId = room.BookedByUserId;
            if (bookedUserId.HasValue) // Hvis bookedUserId har en værdi
            {
                var notification = new Notification
                {
                    UserId = bookedUserId.Value,
                    Message = $"Din booking for lokalet \"{room.RoomName}\" blev annulleret.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Bookingen for lokalet \"{room.RoomName}\" blev annulleret, og brugeren fik en notifikation.";
            return RedirectToPage(new { filter = Filter });
        }
    }
}








