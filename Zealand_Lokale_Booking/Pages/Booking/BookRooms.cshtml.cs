using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Hent lokaler, inkl. bookinger og brugere
            Rooms = _context.Rooms
                .Include(r => r.Bookings.Where(b => b.IsActive && b.Status == "Active"))
                .ThenInclude(b => b.User) // Inkluder brugeroplysninger
                .ToList();
        }

        public IActionResult OnPostBookRoom(int roomId, DateTime bookingDate, TimeSpan startTime, int intervalMinutes)
        {
            var room = _context.Rooms
                .Include(r => r.Bookings.Where(b => b.IsActive && b.Status == "Active"))
                .FirstOrDefault(r => r.RoomId == roomId);

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

            if (room.RoomType == "Klasselokale")
            {
                // Find aktive bookinger for dette lokale på samme dato
                var sameDateBookings = room.Bookings
                    .Where(b => b.StartTime.Date == bookingDate.Date)
                    .ToList();

                // Tillad maks. 2 aktive bookinger på samme dato
                if (sameDateBookings.Count >= 2)
                {
                    TempData["ErrorMessage"] = "Klasselokalet har allerede to aktive bookinger på denne dato.";
                    return RedirectToPage();
                }

                // Kontrollér for overlap med eksisterende bookinger for samme bruger
                var userOverlap = sameDateBookings
                    .Any(b => b.StartTime < endDateTime && b.EndTime > startDateTime && b.UserId == user.UserId);

                if (userOverlap)
                {
                    TempData["ErrorMessage"] = "Du har allerede booket dette lokale i dette tidsrum.";
                    return RedirectToPage();
                }
            }

            // Opret ny booking
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

            // Opdater IsBooked-status for klasselokalet, hvis der er mindst én aktiv booking
            if (room.RoomType == "Klasselokale")
            {
                room.IsBooked = room.Bookings.Any(b => b.IsActive);
            }
            else
            {
                room.IsBooked = true;
                room.BookedByUserId = user.UserId;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Lokalet '{room.RoomName}' blev booket fra {startDateTime:HH:mm} til {endDateTime:HH:mm} den {bookingDate:dd-MM-yyyy}.";
            return RedirectToPage();
        }
    }
}






















































