using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class BookRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BookRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }

        public void OnGet()
        {
            Rooms = _context.Rooms.ToList();
        }

        public IActionResult OnPostBookRoom(int roomId, DateTime bookingDate, TimeSpan startTime, int intervalMinutes)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["ErrorMessage"] = "Lokalet findes ikke.";
                return RedirectToPage();
            }

            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Brugeren kunne ikke findes.";
                return RedirectToPage();
            }

            if (room.RoomType == "Auditorium" && user.RoleId != 2) // Kun lærere kan booke
            {
                TempData["ErrorMessage"] = "Kun lærere kan booke auditoriet.";
                return RedirectToPage();
            }

            var startDateTime = bookingDate.Add(startTime);
            var endDateTime = startDateTime.AddMinutes(intervalMinutes);

            var overlappingBooking = _context.Bookings
                .Where(b => b.RoomId == roomId && b.IsActive)
                .Any(b => b.StartTime < endDateTime && b.EndTime > startDateTime);

            if (overlappingBooking)
            {
                TempData["ErrorMessage"] = "Lokalet er allerede booket i dette tidsrum.";
                return RedirectToPage();
            }

            var booking = new ZealandLokaleBooking.Models.Booking
            {
                RoomId = room.RoomId,
                UserId = user.UserId,
                StartTime = startDateTime,
                EndTime = endDateTime,
                IsDeleted = false,
                IsActive = true,
                Status = "Active"
            };

            _context.Bookings.Add(booking);
            room.IsBooked = true;
            room.BookedByUserId = user.UserId;

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Lokalet '{room.RoomName}' blev booket fra {startDateTime:HH:mm} til {endDateTime:HH:mm} den {bookingDate:dd-MM-yyyy}.";
            return RedirectToPage();
        }
    }
}
























