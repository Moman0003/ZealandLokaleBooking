namespace ZealandLokaleBooking.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public string RoomType { get; set; } = null!;
        public bool IsBooked { get; set; }
        public int? BookedByUserId { get; set; }
        public User? BookedByUser { get; set; }
        public bool IsAuditorium { get; set; }

        // Navigation property til Bookings
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}