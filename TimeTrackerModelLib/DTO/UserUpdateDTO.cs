namespace TimeTrackerModelLib.DTO
{
    public class UserUpdateDTO
    {
        public string? NewPassword { get; set; } // Optional
        public string? PhoneNumber { get; set; } // Optional
        public string? Address { get; set; } // Optional
        public string? ImagePath { get; set; } // Optional
    }
}
