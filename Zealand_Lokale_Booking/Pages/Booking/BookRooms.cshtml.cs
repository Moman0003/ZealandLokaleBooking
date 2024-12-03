using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For at bruge Include
using System.Linq;
using System.Collections.Generic;  
using System;
using System.Threading.Tasks;
using ZealandLokaleBooking.Services;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class BookRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor for at injicere ApplicationDbContext
        public BookRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Liste af lokaler, som vises på siden
        public List<Room> Rooms { get; set; }

        // Liste af bookinger
        public List<ZealandLokaleBooking.Models.Booking> Bookings { get; set; }

        // Hent alle lokaler og bookinger på OnGet
        public void OnGet()
        {
            // Fetch all rooms
            Rooms = _context.Rooms.ToList();

            // Fetch all bookings med relaterede User-data
            Bookings = _context.Bookings
                .Include(b => b.User) // Indlæs User-relationen
                .ToList();
        }

        // Håndterer rum booking
        public IActionResult OnPostBookRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room != null && !room.IsBooked)
            {
                // Mark room as booked
                room.IsBooked = true;

                // Find user based on email and assign to booking
                var userEmail = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    room.BookedByUserId = user.UserId;

                    // Logic for booking based on room type
                    if (room.RoomType == "Klasselokale")
                    {
                        // Logic for full day booking (e.g. 8:00 - 16:00)
                    }
                    else if (room.RoomType == "Gruppelokale")
                    {
                        // Logic for 3-hour booking limit
                    }

                    // Save booking and room state
                    _context.SaveChanges();
                    return RedirectToPage(); // Refresh page after booking
                }
            }

            return RedirectToPage(); // Redirect back if there is no room or user
        }

        // Håndterer annullering af booking
        public async Task<IActionResult> OnPostCancelBookingAsync(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.User) // Indlæs User-relationen
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null || booking.Status == "Cancelled")
            {
                TempData["Error"] = "Booking findes ikke eller er allerede annulleret.";
                return RedirectToPage();
            }

            // Tjek, om annulleringen er inden for tidsrammen (3 dage før start)
            if (booking.StartTime < DateTime.Now.AddDays(3))
            {
                TempData["Error"] = "Booking kan ikke annulleres med mindre end 3 dages varsel.";
                return RedirectToPage();
            }

            // Opdater bookingstatus
            booking.Status = "Cancelled";

            // Find underviseren, der annullerede
            var teacherEmail = User.Identity.Name;
            var teacher = _context.Users.FirstOrDefault(u => u.Email == teacherEmail);
            if (teacher != null)
            {
                booking.CancelledBy = teacher.UserId; // Gem underviserens ID
            }

            // Send notifikation til den studerende
            if (booking.User != null)
            {
                await NotifyStudentAsync(booking.User.Email, booking.Id);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking blev annulleret og notifikation sendt til den studerende.";
            return RedirectToPage();
        }

        private async Task NotifyStudentAsync(string email, int bookingId)
        {
            // Simpel notifikation - eksempelvis en email
            await EmailService.SendEmailAsync(
                email,
                "Din booking er blevet annulleret",
                $"Din booking med ID {bookingId} er blevet annulleret med 3 dages varsel."
            );
        }
    }
}


