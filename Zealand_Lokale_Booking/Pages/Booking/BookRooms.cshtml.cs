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
            // Hent alle lokaler
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

            // Beregn start- og sluttidspunkt for booking
            var startDateTime = bookingDate.Add(startTime);
            var endDateTime = startDateTime.AddMinutes(intervalMinutes);

            // Tilføj regler for specifikke lokaler
            if (room.RoomType == "Auditorium")
            {
                if (user.RoleId != 2) // Kun lærere kan booke
                {
                    TempData["ErrorMessage"] = "Kun lærere kan booke auditoriet.";
                    return RedirectToPage();
                }

                if (intervalMinutes != 30 && intervalMinutes != 60)
                {
                    TempData["ErrorMessage"] = "Auditoriet kan kun bookes i 30- eller 60-minutters intervaller.";
                    return RedirectToPage();
                }
            }
            else if (room.RoomType == "Classroom")
            {
                // Regler for klasselokaler: Book for 1, 2, 3 timer eller hele dagen (480 minutter)
                if (intervalMinutes != 60 && intervalMinutes != 120 && intervalMinutes != 180 && intervalMinutes != 480)
                {
                    TempData["ErrorMessage"] = "Klasselokaler kan kun bookes for 1 time, 2 timer, 3 timer eller hele dagen.";
                    return RedirectToPage();
                }
            }
            else if (room.RoomType == "GroupRoom")
            {
                // Regler for gruppelokaler: Kun 1 eller 2 timer
                if (intervalMinutes != 60 && intervalMinutes != 120)
                {
                    TempData["ErrorMessage"] = "Gruppelokaler kan kun bookes for 1 eller 2 timer.";
                    return RedirectToPage();
                }
            }

            // Kontrollér for overlappende bookinger
            var overlappingBooking = _context.Bookings
                .Where(b => b.RoomId == roomId && b.IsActive)
                .Any(b => b.StartTime < endDateTime && b.EndTime > startDateTime);

            if (overlappingBooking)
            {
                TempData["ErrorMessage"] = "Lokalet er allerede booket i dette tidsrum.";
                return RedirectToPage();
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
            room.IsBooked = true; // Marker lokalet som booket
            room.BookedByUserId = user.UserId;
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Lokalet '{room.RoomName}' blev booket fra {startDateTime:HH:mm} til {endDateTime:HH:mm} den {bookingDate:dd-MM-yyyy}.";
            return RedirectToPage();
        }
    }
}























