namespace ZealandLokaleBooking.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public string RoomType { get; set; } = null!; // Fx "Gruppe", "Møde"
        public bool IsBooked { get; set; } // True, hvis lokalet er booket
    }

}