using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class MyRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> BookedRooms { get; set; } = new List<Room>();

        public void OnGet()
        {
            // Her kan du tilføje logik til at filtrere lokaler booket af den loggede bruger
            BookedRooms = _context.Rooms.Where(r => r.IsBooked).ToList();
        }
    }
}
