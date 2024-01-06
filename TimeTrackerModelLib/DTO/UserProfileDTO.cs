namespace TimeTrackerModelLib.DTO
{
    public class UserProfileDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }

        public string JobPosition { get; set; }

        public string ImagePath { get; set; }
        public DateTime HiredDate { get; set; }
    }
}
