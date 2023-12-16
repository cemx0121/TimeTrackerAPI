using TimeTrackerModelLib.Data;
using TimeTrackerModelLib.Models;
using BCrypt.Net;
using TimeTrackerModelLib.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TimeTrackerAPI.Managers
{
    public class UserManager : IUserManager
    {
        private readonly timetrackerdbContext _dbContext = new timetrackerdbContext();
        private const string SecretKey = "your_longer_secret_key_here_at_least_16_characters";



        public AuthenticatedUserDTO Authenticate(string email, string password)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new ArgumentException("No user is registered with that email!");
            }
            if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid password!");
            }


            var token = GenerateJwtToken(user);
            return new AuthenticatedUserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Token = token
            };
        }

        public User CreateUser(User user, UserProfile userProfile)
        {
            // Check if user with email already exists
            User existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            // Validate email format and domain
            if (!IsValidEmail(user.Email) || !user.Email.EndsWith("@green.ai", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email must be a valid email with the '@green.ai' domain.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.RoleId = 2;

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            userProfile.UserId = user.UserId;
            _dbContext.UserProfiles.Add(userProfile);
            _dbContext.SaveChanges();

            return user;
        }

        public UserProfileDTO GetUserProfile(int userId)
        {
            var userProfile = _dbContext.UserProfiles
                .Include(up => up.User) 
                .FirstOrDefault(up => up.UserId == userId);

            if (userProfile == null)
            {
                throw new ArgumentException("User was not found!");
            }

            return new UserProfileDTO
            {
                Email = userProfile.User?.Email, 
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Address = userProfile.Address,
                PhoneNumber = userProfile.PhoneNumber
                // Map other properties as needed
            };
        }



        private string GenerateJwtToken(User user)
        {
            var role = _dbContext.Roles.FirstOrDefault(r => r.RoleId == user.RoleId)?.Role1; 

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role ?? "Employee") // Default to "Employee" if no role found
    };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}