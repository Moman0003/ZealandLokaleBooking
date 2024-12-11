using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq; 
using System.Collections.Generic;  // Sørg for at importere for at bruge List<> og andre samlinger
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
                // Find user based on email and assign to booking
                var userEmail = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    // Tjek om brugeren allerede har 3 aktive bookinger
                    var activeBookingsCount = _context.Bookings
                        .Where(b => b.UserId == user.UserId && b.IsActive && !b.IsDeleted)
                        .Count();

                    if (activeBookingsCount >= 3)
                    {
                        TempData["ErrorMessage"] = "Du kan kun have 3 aktive bookinger ad gangen.";
                        return RedirectToPage(); // Redirect tilbage med fejlbesked
                    }

                    // Mark room as booked
                    room.IsBooked = true;
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

                    // Opret en ny booking
                    var booking = new ZealandLokaleBooking.Models.Booking()
                    {
                        RoomId = room.RoomId,
                        UserId = user.UserId,
                        StartTime = DateTime.Now, // Placeholder, dette kan ændres til faktisk starttid
                        EndTime = DateTime.Now.AddHours(3), // Placeholder, dette kan ændres baseret på rumtypen
                        IsDeleted = false,
                        IsActive = true
                    };

                    // Tilføj booking til databasen
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();
                    return RedirectToPage(); // Refresh page after booking
                }
            }

            return RedirectToPage(); // Redirect back if there is no room or user
        }
    }
}
