using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace Zealand_Lokale_Booking.Pages.Booking
{
    public class BookRoomsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BookRoomsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> AvailableRooms { get; set; } = new List<Room>();

        public void OnGet()
        {
            // Hent alle ledige lokaler
            AvailableRooms = _context.Rooms.Where(r => !r.IsBooked).ToList();
        }

        public IActionResult OnPost(int RoomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            if (room != null)
            {
                room.IsBooked = true;
                _context.SaveChanges();
            }

            return RedirectToPage("/Booking/BookRooms");
        }
    }
}