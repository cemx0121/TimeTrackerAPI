using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPI.Managers
{
    public interface IWorkShiftManager
    {
        List<WorkShiftDTO> GetAllWorkShifts(DateTime date);

        void AddWorkShift(WorkShiftDTO workShiftDTO);

        void UpdateWorkShift(int workShiftId, WorkShiftDTO workShiftDTO);
        void DeleteWorkShift(int workShiftId);

        List<WorkShiftDTO> GetWorkShiftsForWeek(int year, int weekNumber, int userId);

    }
}
