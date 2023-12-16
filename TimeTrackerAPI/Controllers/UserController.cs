using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Managers;
using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserManager userMgr = new UserManager();


        // POST: api/User/authenticate
        [HttpPost("login")]
        public ActionResult<AuthenticatedUserDTO> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                AuthenticatedUserDTO authUser = userMgr.Authenticate(loginDto.Email, loginDto.Password);
                return Ok(authUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("profile/{userId}")]
        //[Authorize(Roles = "Employee")]
        [Authorize]
        public ActionResult<UserProfileDTO> GetUserProfile(int userId)
        {
            try
            {
                UserProfileDTO userProfile = userMgr.GetUserProfile(userId);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/User/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDTO> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                UserProfile userProfile = new UserProfile
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Address = registerDto.Address,
                    PhoneNumber = registerDto.PhoneNumber
                };

                User newUser = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = registerDto.Password
                };

                User createdUser = userMgr.CreateUser(newUser, userProfile);

                UserDTO userDto = new UserDTO
                {
                    UserId = createdUser.UserId,
                    Email = createdUser.Email
                };

                return CreatedAtAction("Register", new { id = userDto.UserId }, userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}