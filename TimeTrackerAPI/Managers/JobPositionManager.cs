using TimeTrackerModelLib.Data;
using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPI.Managers
{
    public class JobPositionManager : IJobPositionManager
    {
        private readonly dbContext _dbContext = new dbContext();

        public List<JobPositionDTO> GetAllJobPositions()
        {
            List<JobPositionDTO> jobPositionsList = _dbContext.JobPositions
                .Select(jp => new JobPositionDTO
                {
                    JobPositionId = jp.JobPositionId,
                    JobPosition = jp.Position
                })
                .ToList();

            return jobPositionsList;
        }

    }
}
