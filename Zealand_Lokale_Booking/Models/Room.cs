namespace ZealandLokaleBooking.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public string RoomType { get; set; }
        public bool IsBooked { get; set; }

        // Ny fremmednøgle
        public int? BookedByUserId { get; set; }
        public User BookedByUser { get; set; } // Navigation property
    }

}