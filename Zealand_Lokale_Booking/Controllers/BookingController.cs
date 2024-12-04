using Microsoft.AspNetCore.Mvc;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using System.Linq;
namespace ZealandLokaleBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
       
        // POST: Booking/BookRoom
        [HttpPost]
        public IActionResult BookRoom(int roomId, DateTime startTime, DateTime endTime)
        {
            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail); 
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            if (room.IsBooked)
            {
                // Hvis lokalet allerede er booket
                TempData["ErrorMessage"] = "Lokalet er allerede booket.";
                return RedirectToPage("/Booking/BookRooms");
            }

            // Opret en ny booking
            var booking = new Booking
            {
                RoomId = room.RoomId,
                UserId = user.UserId,
                StartTime = startTime,
                EndTime = endTime,
                
                IsDeleted = false,
                IsActive = true
            };
            // Tilføj booking til databasen
            _context.Bookings.Add(booking);
            room.IsBooked = true;  // Markér lokalet som booket
            _context.SaveChanges();
            return RedirectToPage("/StudentDashboard"); // Omdirigér efter booking
        }
        
        // GET: Booking/ConfirmDelete/5
        public IActionResult ConfirmDelete(int id)
        {
            var booking = _context.Bookings.Where(b => b.Id == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _context.Bookings.Where(b => b.Id == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }
            // Opdater Room tabel for at markere det som ledigt
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room != null)
            {
                room.IsBooked = false; // Markere lokalet som ledigt
                room.BookedByUserId = null; // Fjern den bookede bruger
                _context.SaveChanges();
            }
            // Slet booking posten
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", "StudentDashboard"); // Redirect til student dashboard
        }
    }
}
