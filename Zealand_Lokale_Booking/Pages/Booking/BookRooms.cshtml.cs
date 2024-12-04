using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq; 
using System.Collections.Generic;  // Sørg for at importere for at bruge List<> og andre samlinger
using System;
using System.Text.RegularExpressions;


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
        

        public void OnGet()
        {
            Rooms = _context.Rooms
                .AsEnumerable() // Hent alle data til klienten, så vi kan bruge Regex
                .OrderBy(r => r.RoomType == "Gruppe" ? 0 : 1) // Ensure Gruppelokaler come first
                .ThenBy(r => r.RoomType == "Klasselokale" ? int.Parse(Regex.Match(r.RoomName, @"\d+").Value) : int.MaxValue) // Extract number for Klasselokale
                .ThenBy(r => r.RoomName) // Sort by RoomName if numbers are the same
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
    }
}
