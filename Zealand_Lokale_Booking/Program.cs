using Microsoft.EntityFrameworkCore;
using ZealandLokaleBooking.Data;
using ZealandLokaleBooking.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Tilføj services til containeren
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrér EmailService som en service
builder.Services.AddScoped<EmailService>();

// Tilføj autentificering
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect til login, hvis ikke logget ind
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect ved adgang nægtet
    });

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Aktiver autentificering
app.UseAuthorization();  // Aktiver autorisation

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages og andre ruter
app.MapRazorPages();

app.Run();