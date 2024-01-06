using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeTrackerModelLib.Data;
using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Managers
{
    public class UserManager : IUserManager
    {
        private readonly dbContext _dbContext = new dbContext();
        private const string SecretKey = "your_longer_secret_key_here_at_least_16_characters";

        public (UserProfileDTO userProfile, string token) Authenticate(string email, string password)
        {
            User user = _dbContext.Users
                                  .Include(u => u.UserProfiles)
                                    .ThenInclude(up => up.JobPosition)
                                  .Include(u => u.Role)
                                  .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                throw new ArgumentException("No user is registered with that email!");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid password!");
            }

            UserProfile userProfileEntity = user.UserProfiles.FirstOrDefault();

            UserProfileDTO userProfile = new UserProfileDTO
            {
                UserId = user.UserId,
                FirstName = userProfileEntity.FirstName,
                LastName = userProfileEntity.LastName,
                Address = userProfileEntity.Address,
                PhoneNumber = userProfileEntity.PhoneNumber,
                Email = user.Email,
                UserRole = user.Role?.UserRole,
                JobPosition = userProfileEntity.JobPosition?.Position,
                ImagePath = userProfileEntity.ImagePath,
                HiredDate = userProfileEntity.HiredDate
            };


            string token = GenerateJwtToken(user);

            return (userProfile, token);
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

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            userProfile.UserId = user.UserId;
            _dbContext.UserProfiles.Add(userProfile);
            _dbContext.SaveChanges();

            return user;
        }

        public UserProfileDTO GetUserProfileByEmail(string email)
        {
            User user = _dbContext.Users
                              .Include(u => u.UserProfiles)
                                .ThenInclude(up => up.JobPosition)
                              .Include(u => u.Role)
                              .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                throw new ArgumentException("User with this email was not found!");
            }

            UserProfile userProfileEntity = user.UserProfiles.FirstOrDefault();
            UserProfileDTO userProfile = null;

            return new UserProfileDTO
            {
                UserId = user.UserId,
                FirstName = userProfileEntity.FirstName,
                LastName = userProfileEntity.LastName,
                Address = userProfileEntity.Address,
                PhoneNumber = userProfileEntity.PhoneNumber,
                Email = user.Email,
                UserRole = user.Role?.UserRole,
                JobPosition = userProfileEntity.JobPosition?.Position,
                ImagePath = userProfileEntity.ImagePath,
                HiredDate = userProfileEntity.HiredDate
            };
        }


        public void UpdateUserRoleAndJobPosition(string email, UserUpdateAdminDTO updateDto)
        {
            User user = GetUserByEmail(email);

            // Update role if provided
            if (updateDto.RoleId.HasValue)
            {
                user.RoleId = updateDto.RoleId.Value;
            }

            // Update job position if provided
            UserProfile userProfile = user.UserProfiles.FirstOrDefault();
            if (userProfile != null && updateDto.JobPositionId.HasValue)
            {
                userProfile.JobPositionId = updateDto.JobPositionId.Value;
            }

            _dbContext.SaveChanges();
        }



        public void UpdateUserProfile(string email, UserUpdateDTO updateDto)
        {
            User user = GetUserByEmail(email);

            // Update password if provided
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            }

            UpdateUserProfileDetails(user, updateDto);

            _dbContext.SaveChanges();
        }

        public List<UserProfileDTO> GetAllUserProfiles()
        {
            var users = _dbContext.Users
                                  .Include(u => u.UserProfiles)
                                    .ThenInclude(up => up.JobPosition)
                                  .Include(u => u.Role)
                                  .ToList();

            var userProfilesDto = users.SelectMany(u => u.UserProfiles, (u, up) => new UserProfileDTO
            {
                UserId = u.UserId,
                FirstName = up.FirstName,
                LastName = up.LastName,
                Address = up.Address,
                PhoneNumber = up.PhoneNumber,
                Email = u.Email,
                UserRole = u.Role?.UserRole,
                JobPosition = up.JobPosition?.Position,
                ImagePath = up.ImagePath,
                HiredDate = up.HiredDate
            }).ToList();

            return userProfilesDto;
        }

        public void DeleteUserById(int userId)
        {
            User user = _dbContext.Users
                                     .Include(u => u.UserProfiles)
                                     .Include(u => u.WorkShifts)
                                     .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User was not found with this user id");
            }

            // Delete work shifts and user profiles associated with the user
            _dbContext.WorkShifts.RemoveRange(user.WorkShifts);
            _dbContext.UserProfiles.RemoveRange(user.UserProfiles);

            // Delete the user
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }






        private User GetUserByEmail(string email)
        {
            return _dbContext.Users
                             .Include(u => u.UserProfiles)
                             .FirstOrDefault(u => u.Email.ToLower() == email.ToLower())
                             ?? throw new ArgumentException("User not found!");
        }

        private void UpdateUserProfileDetails(User user, UserUpdateDTO updateDto)
        {
            UserProfile userProfile = user.UserProfiles.FirstOrDefault();
            if (userProfile != null)
            {
                // Update phone number if provided
                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                {
                    userProfile.PhoneNumber = updateDto.PhoneNumber;
                }

                // Update address if provided
                if (!string.IsNullOrEmpty(updateDto.Address))
                {
                    userProfile.Address = updateDto.Address;
                }

                // Update image path if provided
                if (!string.IsNullOrEmpty(updateDto.ImagePath))
                {
                    userProfile.ImagePath = updateDto.ImagePath;
                }
            }
            else
            {
                throw new InvalidOperationException("UserProfile not found for the user.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            string role = _dbContext.Roles.FirstOrDefault(r => r.RoleId == user.RoleId)?.UserRole;

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role ?? "User") // Default to "User" if no role found
    };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
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