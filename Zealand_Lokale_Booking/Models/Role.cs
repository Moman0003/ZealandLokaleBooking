namespace ZealandLokaleBooking.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } // fx "Studerende", "Underviser"
        public ICollection<User> Users { get; set; } // Relation til brugere
    }
}

