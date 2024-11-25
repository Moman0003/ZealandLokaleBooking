namespace ZealandLokaleBooking.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } // Navigation property
        public ICollection<Booking> Bookings { get; set; } // Relation til bookinger
    }
}