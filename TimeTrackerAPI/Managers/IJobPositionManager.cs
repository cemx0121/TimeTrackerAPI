using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPI.Managers
{
    public interface IJobPositionManager
    {
        List<JobPositionDTO> GetAllJobPositions();
    }
}
