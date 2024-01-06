using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Managers;
using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPositionsController : ControllerBase
    {
        private IJobPositionManager jobPositionMgr = new JobPositionManager();

        [HttpGet()]
        public ActionResult<List<JobPositionDTO>> GetAllUserProfiles()
        {
            try
            {
                var jobPositions = jobPositionMgr.GetAllJobPositions();
                return Ok(jobPositions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
