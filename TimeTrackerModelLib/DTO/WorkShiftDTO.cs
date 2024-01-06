namespace TimeTrackerModelLib.DTO
{
    public class WorkShiftDTO
    {
        public int WorkShiftId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }

    }
}
