using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace TimeTrackerAPI.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserManager> mockUserManager;
        private UserController userController;

        [SetUp]
        public void Setup()
        {
            // Arrange
            mockUserManager = new Mock<IUserManager>();
            userController = new UserController(mockUserManager.Object);
        }

        [Test]
        public void Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "user@example.com", Password = "password" };
            mockUserManager.Setup(m => m.Authenticate(loginDto.Email, loginDto.Password))
                           .Returns((new UserProfileDTO(), "validToken"));

            // Act
            var result = userController.Login(loginDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "user@example.com", Password = "wrongPassword" };
            mockUserManager.Setup(m => m.Authenticate(loginDto.Email, loginDto.Password))
                           .Throws(new ArgumentException("Invalid credentials"));

            // Act
            var result = userController.Login(loginDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
