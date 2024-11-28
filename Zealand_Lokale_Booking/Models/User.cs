namespace ZealandLokaleBooking.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // Tilføj denne linje
    }
}