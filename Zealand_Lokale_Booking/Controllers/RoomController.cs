using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using System.Linq;
using System; 


namespace ZealandLokaleBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AvailableRooms()
        {
            // Find ledige lokaler ved at kontrollere, om der er overlappende bookinger
            var availableRooms = _context.Rooms
                .Where(r => !_context.Bookings
                    .Any(b => b.RoomId == r.RoomId && b.EndTime > DateTime.UtcNow)) // Brug DateTime.UtcNow
                .ToList();

            return View(availableRooms);
        }

        [HttpGet]
        public IActionResult BookRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room == null)
            {
                return NotFound("Lokalet findes ikke.");
            }

            return View(room);
        }

        [HttpPost]
        public IActionResult BookRoom(int roomId, DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
            {
                ViewBag.ErrorMessage = "Sluttidspunkt skal være efter starttidspunkt.";
                var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
                return View(room);
            }

            // Kontrollér overlap med eksisterende bookinger
            var isBooked = _context.Bookings.Any(b =>
                b.RoomId == roomId &&
                ((startTime < b.EndTime && startTime >= b.StartTime) ||
                 (endTime > b.StartTime && endTime <= b.EndTime)));

            if (isBooked)
            {
                ViewBag.ErrorMessage = "Lokalet er allerede booket i dette tidsrum.";
                var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
                return View(room);
            }

            // Hent bruger-ID fra claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("Brugeren er ikke logget ind.");
            }

            // Opret en ny booking
            var booking = new Booking
            {
                RoomId = roomId,
                StartTime = startTime,
                EndTime = endTime,
                UserId = int.Parse(userIdClaim.Value) // Sørg for, at dette felt findes
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("AvailableRooms");
        }
    }
}