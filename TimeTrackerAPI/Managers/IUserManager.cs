using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Managers
{
    public interface IUserManager
    {
        (UserProfileDTO userProfile, string token) Authenticate(string email, string password);
        User CreateUser(User user, UserProfile userProfile);

        UserProfileDTO GetUserProfileByEmail(string email);

        void UpdateUserRoleAndJobPosition(string email, UserUpdateAdminDTO updateDto);
        void UpdateUserProfile(string email, UserUpdateDTO updateDto);

        List<UserProfileDTO> GetAllUserProfiles();

        void DeleteUserById(int id);
    }
}
