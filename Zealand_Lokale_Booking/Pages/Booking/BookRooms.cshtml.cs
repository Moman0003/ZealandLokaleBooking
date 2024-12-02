using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            // Fetch all rooms
            Rooms = _context.Rooms.ToList();
            
            {
                ViewData["ShowBackButton"] = "true";
                ViewData["BackUrl"] = "/StudentDashboard";
            }
        }

        public IActionResult OnPostBookRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room != null && !room.IsBooked)
            {
                room.IsBooked = true;

                // Tildel brugeren, der bookede lokalet
                room.BookedByUserId = _context.Users
                    .FirstOrDefault(u => u.Email == User.Identity.Name)?.UserId;

                // Valider booking baseret på type
                if (room.RoomType == "Klasselokale")
                {
                    // Logik for at booke hele dagen (fx fra 8-16)
                }
                else if (room.RoomType == "Gruppelokale")
                {
                    // Logik for at begrænse booking til 3 timer
                }

                _context.SaveChanges();
            }

            return RedirectToPage(); // Genindlæs siden efter booking
        }
        
        
        
    }
}