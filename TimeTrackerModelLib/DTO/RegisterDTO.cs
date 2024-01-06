namespace TimeTrackerModelLib.DTO
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int JobPositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HiredDate { get; set; }
        public string ImagePath { get; set; }
        public int RoleId { get; set; }

    }
}
