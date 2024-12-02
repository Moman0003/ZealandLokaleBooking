using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Zealand_Lokale_Booking.Pages;

public class About : PageModel
{
    public void OnGet()
    {
        ViewData["ShowBackButton"] = "true";
        ViewData["BackUrl"] = "/StudentDashboard";
    }
}