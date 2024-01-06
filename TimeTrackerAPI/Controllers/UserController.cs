using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrackerAPI.Managers;
using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userMgr = new UserManager();


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var (userProfile, token) = userMgr.Authenticate(loginDto.Email, loginDto.Password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // Overwrite the cookie with an expired one
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1) // Set the expiration to a past time
            };
            Response.Cookies.Append("AuthToken", "", cookieOptions);

            // Then delete the cookie
            Response.Cookies.Delete("AuthToken");
            return Ok(new { SuccessMessage = "Logged out successfully" });
        }

        // Used for the front-end to call at every page refresh/reload to check if the credentials are valid and thus stay logged in
        [HttpGet("validate-auth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            // Extract email from the JWT token
            Claim emailClaim = HttpContext.User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized("Your authorization could not be validated, please login again!");
            }

            string email = emailClaim.Value;
            try
            {
                UserProfileDTO userProfile = userMgr.GetUserProfileByEmail(email);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpGet("profile/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = "Employee")]
        [Authorize]
        public ActionResult<UserProfileDTO> GetUserProfile(string email)
        {
            try
            {
                UserProfileDTO userProfile = userMgr.GetUserProfileByEmail(email);
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/User/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public IActionResult Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                UserProfile userProfile = new UserProfile
                {
                    JobPositionId = registerDto.JobPositionId,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Address = registerDto.Address,
                    PhoneNumber = registerDto.PhoneNumber,
                    HiredDate = registerDto.HiredDate,
                    ImagePath = registerDto.ImagePath,

                };

                User newUser = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = registerDto.Password,
                    RoleId = registerDto.RoleId
                };

                User createdUser = userMgr.CreateUser(newUser, userProfile);


                return Ok(new { SuccessMessage = $"User {registerDto.Email} was successfully registered!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/profile")]
        [Authorize]
        public IActionResult UpdateUserProfile([FromBody] UserUpdateDTO updateDto)
        {
            try
            {
                string email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                userMgr.UpdateUserProfile(email, updateDto);
                return Ok(new { Message = "Your user profile was updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/role/{email}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUserRoleAndJobPosition(string email, [FromBody] UserUpdateAdminDTO updateDto)
        {
            try
            {
                userMgr.UpdateUserRoleAndJobPosition(email, updateDto);
                return Ok("User role and job position updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all-profiles")]
        [Authorize]
        public ActionResult<List<UserProfileDTO>> GetAllUserProfiles()
        {
            try
            {
                var userProfiles = userMgr.GetAllUserProfiles();
                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                userMgr.DeleteUserById(userId);
                return Ok("User and related data deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}