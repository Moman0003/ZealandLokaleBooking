namespace ZealandLokaleBooking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; } // Tilføj denne linje
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Room Room { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}