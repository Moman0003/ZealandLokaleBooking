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

        // GET: Booking/ConfirmDelete/5
        public IActionResult ConfirmDelete(int id)
        {
            var booking = _context.Bookings.Where(b => b.BookingId == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _context.Bookings.Where(b => b.BookingId == id).FirstOrDefault();
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", "StudentDashboard");
        }
    }
}