namespace ZealandLokaleBooking.Models
{
    public class Booking
    {
        public int Id { get; set; } // Brug 'Id' i stedet for 'BookingId'
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Room Room { get; set; } = null!;
        public User User { get; set; } = null!;

        // Ny property for dato
        public DateTime Date => StartTime.Date; // Udleder kun datoen fra StartTime
        
        // Tilføj de manglende properties
        public bool IsDeleted { get; set; } = false;  // Initialiserer som false
        public bool IsActive { get; set; } = true;    // Initialiserer som true
    }
}