using Microsoft.AspNetCore.Mvc.RazorPages;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Models;
using System.Linq;

namespace Zealand_Lokale_Booking.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User CurrentUser { get; set; } // Omdøbt til CurrentUser for at undgå konflikt

        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                CurrentUser = _context.Users.FirstOrDefault(u => u.Email == email);
            }
        }
    }
}