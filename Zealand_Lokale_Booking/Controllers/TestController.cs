using Microsoft.AspNetCore.Mvc;
using ZealandLokaleBooking.Data;

namespace ZealandLokaleBooking.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms.ToList();
            return Ok(rooms); // Returnerer en liste over lokaler i JSON-format
        }
    }
}

