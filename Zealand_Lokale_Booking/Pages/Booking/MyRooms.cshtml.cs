using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using System.Linq;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class MyRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> BookedRooms { get; set; }

        public void OnGet()
        {
            var userEmail = User.Identity.Name;

            // Find the UserId of the currently logged-in user
            var currentUserId = _context.Users
                .FirstOrDefault(u => u.Email == userEmail)?.UserId;

            // Fetch rooms booked by the logged-in user
            if (currentUserId != null)
            {
                BookedRooms = _context.Rooms
                    .Where(r => r.BookedByUserId == currentUserId)
                    .ToList();
            }
            else
            {
                BookedRooms = new List<Room>(); // No rooms if user is not found
            }
        }

        public IActionResult OnPostDeleteRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room != null)
            {
                room.IsBooked = false; // Mark the room as available
                room.BookedByUserId = null; // Clear the booking

                _context.SaveChanges();
            }

            return RedirectToPage(); // Reload the page after deletion
        }
    }
}