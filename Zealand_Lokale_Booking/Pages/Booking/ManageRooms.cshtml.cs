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
                // Vis kun lokaler med aktive bookinger
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .Include(r => r.Bookings.Where(b => b.IsActive && b.Status == "Active"))
                    .Where(r => r.Bookings.Any(b => b.IsActive && b.Status == "Active"))
                    .ToList();
            }
            else if (filter == "available")
            {
                // Vis kun lokaler uden aktive bookinger
                Rooms = _context.Rooms
                    .Include(r => r.Bookings)
                    .Where(r => !r.Bookings.Any(b => b.IsActive && b.Status == "Active"))
                    .ToList();
            }
            else // "all"
            {
                // Vis alle lokaler, og medtag eventuelle aktive bookinger
                Rooms = _context.Rooms
                    .Include(r => r.BookedByUser)
                    .Include(r => r.Bookings.Where(b => b.IsActive && b.Status == "Active"))
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int roomId)
        {
            var room = _context.Rooms
                .Include(r => r.BookedByUser)
                .Include(r => r.Bookings)
                .FirstOrDefault(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["ErrorMessage"] = "Lokalet blev ikke fundet.";
                return RedirectToPage(new { filter = Filter });
            }

            // Find den aktive booking for det specifikke rum
            var booking = room.Bookings.FirstOrDefault(b => b.IsActive && b.Status == "Active");
            if (booking != null)
            {
                booking.Status = "Cancelled";
                booking.IsActive = false;

                // Hvis der stadig er aktive bookinger for lokalet, behold status
                if (room.Bookings.Any(b => b.IsActive && b.Status == "Active"))
                {
                    room.IsBooked = true;
                }
                else
                {
                    room.IsBooked = false;
                    room.BookedByUserId = null;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Ingen aktive bookinger fundet for dette lokale.";
                return RedirectToPage(new { filter = Filter });
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




































