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

        // Constructor for at injicere ApplicationDbContext
        public BookRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Liste af lokaler, som vises på siden
        public List<Room> Rooms { get; set; }

        // Hent alle lokaler på OnGet
        public void OnGet()
        {
            // Fetch all rooms
            Rooms = _context.Rooms.ToList();
        }

        // Håndterer rum booking
        public IActionResult OnPostBookRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room != null && !room.IsBooked)
            {
                var userEmail = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    var activeBookingsCount = _context.Bookings
                        .Where(b => b.UserId == user.UserId && b.IsActive && !b.IsDeleted)
                        .Count();

                    if (activeBookingsCount >= 3)
                    {
                        TempData["ErrorMessage"] = "Du kan kun have 3 aktive bookinger ad gangen.";
                        return RedirectToPage();
                    }

                    room.IsBooked = true;
                    room.BookedByUserId = user.UserId;

                    var booking = new ZealandLokaleBooking.Models.Booking()
                    {
                        RoomId = room.RoomId,
                        UserId = user.UserId,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now.AddHours(3),
                        IsDeleted = false,
                        IsActive = true
                    };

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();
                    return RedirectToPage();
                }
            }

            return RedirectToPage();
        }
    }
}
