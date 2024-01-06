using System.Globalization;
using TimeTrackerModelLib.Data;
using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Managers
{
    public class WorkShiftManager : IWorkShiftManager
    {

        private readonly dbContext _dbContext = new dbContext();

        public List<WorkShiftDTO> GetAllWorkShifts(DateTime date)
        {
            var workShiftsList = _dbContext.WorkShifts
                .Where(ws => ws.Date.Date == date.Date)
                .Select(ws => new WorkShiftDTO
                {
                    WorkShiftId = ws.WorkShiftId,
                    UserId = ws.UserId,
                    Date = ws.Date,
                    StartTime = ws.StartTime,
                    EndTime = ws.EndTime,
                    Location = ws.Location
                })
                .ToList();

            return workShiftsList;
        }

        public void AddWorkShift(WorkShiftDTO workShiftDTO)
        {
            WorkShift existingWorkShift = _dbContext.WorkShifts
        .FirstOrDefault(ws => ws.UserId == workShiftDTO.UserId && ws.Date.Date == workShiftDTO.Date.Date);

            if (existingWorkShift != null)
            {
                throw new ArgumentException("A work shift for this user on this date already exists.");
            }

            WorkShift workShift = new WorkShift
            {
                UserId = workShiftDTO.UserId,
                Date = workShiftDTO.Date,
                StartTime = workShiftDTO.StartTime,
                EndTime = workShiftDTO.EndTime,
                Location = workShiftDTO.Location
            };

            _dbContext.WorkShifts.Add(workShift);
            _dbContext.SaveChanges();
        }

        public void UpdateWorkShift(int workShiftId, WorkShiftDTO workShiftDTO)
        {
            var workShift = _dbContext.WorkShifts.Find(workShiftId);
            if (workShift != null)
            {
                workShift.StartTime = workShiftDTO.StartTime;
                workShift.EndTime = workShiftDTO.EndTime;
                workShift.Location = workShiftDTO.Location;

                _dbContext.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException("WorkShift not found.");
            }
        }

        public void DeleteWorkShift(int workShiftId)
        {
            var workShift = _dbContext.WorkShifts.Find(workShiftId);
            if (workShift != null)
            {
                _dbContext.WorkShifts.Remove(workShift);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException("WorkShift not found.");
            }
        }

        public List<WorkShiftDTO> GetWorkShiftsForWeek(int year, int weekNumber, int userId)
        {
            var startDate = FirstDateOfWeekISO8601(year, weekNumber);
            var endDate = startDate.AddDays(7);

            return _dbContext.WorkShifts
                .Where(ws => ws.UserId == userId && ws.Date >= startDate && ws.Date < endDate)
                .Select(ws => new WorkShiftDTO
                {
                    WorkShiftId = ws.WorkShiftId,
                    UserId = ws.UserId,
                    Date = ws.Date,
                    StartTime = ws.StartTime,
                    EndTime = ws.EndTime,
                    Location = ws.Location
                })
                .ToList();
        }

        private static DateTime FirstDateOfWeekISO8601(int year, int weekNumber)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)DayOfWeek.Thursday - (int)jan1.DayOfWeek;

            // Adjust days offset if the year starts on Friday, Saturday or Sunday
            if (daysOffset < 0) daysOffset += 7;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Calculate the start date of the week
            DateTime firstDayOfWeek = firstThursday.AddDays((weekNumber - firstWeek) * 7);

            // Adjust to Monday (start of the week)
            return firstDayOfWeek.AddDays(-3);
        }

    }
}
