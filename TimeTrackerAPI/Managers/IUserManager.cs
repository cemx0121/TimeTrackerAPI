using TimeTrackerModelLib.DTO;
using TimeTrackerModelLib.Models;

namespace TimeTrackerAPI.Managers
{
    public interface IUserManager
    {
        AuthenticatedUserDTO Authenticate(string email, string password);
        User CreateUser(User user, UserProfile userProfile);

        UserProfileDTO GetUserProfile(int userId);
    }
}
