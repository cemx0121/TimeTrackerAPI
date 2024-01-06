using System.IdentityModel.Tokens.Jwt;
using TimeTrackerAPI.Managers;
using TimeTrackerModelLib.DTO;

namespace TimeTrackerAPITest
{
    [TestClass]
    public class UserControllerTests
    {
        private IUserManager userMgr = new UserManager();

        [TestMethod]
        public void TestValidLoginReturnedUserAndToken()
        {
            // Arrange
            int expectedUserId = 24;

            // Act
            var authenticationResult = userMgr.Authenticate("cemturan@green.ai", "Cemo2610");
            UserProfileDTO userProfile = authenticationResult.userProfile;
            int actualUserId = userProfile.UserId;
            string token = authenticationResult.token;

            // Assert UserID
            Assert.AreEqual(expectedUserId, actualUserId);

            // Validate Token and decrypting token, so its ready to be read
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);


            // Checking if the token recieved with successfull login claims is matching the actual user.
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "cemturan@green.ai"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == JwtRegisteredClaimNames.NameId && c.Value == actualUserId.ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No user is registered with that email!")]
        public void AuthenticateWithInvalidEmail_ThrowsArgumentException()
        {
            // Act
            userMgr.Authenticate("invalidemail@example.com", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid password!")]
        public void AuthenticateWithInvalidPassword_ThrowsArgumentException()
        {
            // Act
            userMgr.Authenticate("cemturan@green.ai", "wrongPassword");
        }
    }


}