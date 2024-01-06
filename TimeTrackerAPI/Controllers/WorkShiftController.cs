using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrackerAPI.Managers;
using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkShiftController : ControllerBase
    {
        private IWorkShiftManager workShiftMgr = new WorkShiftManager();


        [HttpGet("{date}")]
        [Authorize]
        public ActionResult<List<WorkShiftDTO>> GetAllWorkShifts(DateTime date)
        {
            try
            {
                var workShiftsList = workShiftMgr.GetAllWorkShifts(date);
                return Ok(workShiftsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddWorkShift([FromBody] WorkShiftDTO workShiftDTO)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null && workShiftDTO.UserId.ToString() == userId)
                {
                    workShiftMgr.AddWorkShift(workShiftDTO);
                    return Ok(new { SuccessMessage = "Your work shift has succesfully been registered!" });
                }
                return Unauthorized("Only a logged in/authorized user can put in work shifts for themself");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{workShiftId}")]
        [Authorize]
        public IActionResult UpdateWorkShift(int workShiftId, [FromBody] WorkShiftDTO workShiftDTO)
        {
            try
            {
                workShiftMgr.UpdateWorkShift(workShiftId, workShiftDTO);
                return Ok(new { SuccessMessage = "Your work shift was succesfully updated!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{workShiftId}")]
        [Authorize]
        public IActionResult DeleteWorkShift(int workShiftId)
        {
            try
            {
                workShiftMgr.DeleteWorkShift(workShiftId);
                return Ok(new { SuccessMessage = "Your work shift was succesfully deleted!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("week/{year}/{weekNumber}")]
        [Authorize]
        public ActionResult<List<WorkShiftDTO>> GetWorkShiftsForWeek(int year, int weekNumber)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var workShiftsList = workShiftMgr.GetWorkShiftsForWeek(year, weekNumber, userId);
                return Ok(workShiftsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("week/{year}/{weekNumber}/{userId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<WorkShiftDTO>> GetEmployeeWorkShiftsForWeek(int year, int weekNumber, int userId)
        {
            try
            {
                var workShiftsList = workShiftMgr.GetWorkShiftsForWeek(year, weekNumber, userId);
                return Ok(workShiftsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
