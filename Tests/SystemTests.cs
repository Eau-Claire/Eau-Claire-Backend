using FishFarm.BusinessObjects;
using FishFarm.Repositories;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace SystemTests
{

    [TestClass]
    public class SystemTests
    {
        private UserService _service = null!;
        private Mock<IUserRepository> _userRepo = null!;
        private Mock<IUserProfileService> _profileService = null!;
        private Mock<IDeviceService> _deviceService = null!;
        private Mock<IRefreshTokenService> _refreshTokenService = null!;
        private IMemoryCache _cache = null!;

        [TestInitialize]
        public void Init()
        {
            _userRepo = new Mock<IUserRepository>();
            _profileService = new Mock<IUserProfileService>();
            _deviceService = new Mock<IDeviceService>();
            _refreshTokenService = new Mock<IRefreshTokenService>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _service = new UserService(
                _cache,
                _userRepo.Object,
                _profileService.Object,
                _deviceService.Object,
                _refreshTokenService.Object
            );

            Environment.SetEnvironmentVariable("Jwt__Key", "vT9kY3qL4FxP8mS1H0bN7cW2QpR5uJ6tZyA3KdF8MvB2TxQ1");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
            Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        }

        [TestMethod]
        public void Login_Success_Test()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "test", PasswordHash = "pass", Email = "a@gmail.com" };
            var profile = new UserProfile { FullName = "Test User" };

            _userRepo.Setup(x => x.Login("test", "pass")).Returns(user);
            _profileService.Setup(x => x.GetUserProfile(1)).Returns(profile);
            _deviceService.Setup(x => x.CheckDeviceIsVerified("device1", 1)).Returns(true);
            _refreshTokenService.Setup(x => x.SaveRefreshToken(1, It.IsAny<string>(), It.IsAny<DateTime>())).Returns(true);

            // Act
            var result = _service.Login("test", "pass", "device1");

            // Assert
            Assert.IsNotNull(result);
            Console.WriteLine(result.message);
            Assert.AreEqual("200", result.status);
            Assert.IsTrue(result.isDeviceVerified);
            Assert.IsNotNull(result.accessToken);
            Assert.IsNotNull(result.refreshToken);
        }

        [TestMethod]
        public void Login_InvalidDevice_Return401()
        {
            var user = new User { UserId = 1, Username = "test" };
            var profile = new UserProfile();

            _userRepo.Setup(x => x.Login("test", "pass")).Returns(user);
            _profileService.Setup(x => x.GetUserProfile(1)).Returns(profile);
            _deviceService.Setup(x => x.CheckDeviceIsVerified("dev1", 1)).Returns(false);

            var result = _service.Login("test", "pass", "dev1");

            Assert.AreEqual("401", result.status);
            Assert.IsFalse(result.isDeviceVerified);
        }

        [TestMethod]
        public void RefreshToken_Valid_Return200()
        {
            var response = new LoginResponse { status = "200" };
            var user = new User
            {
                UserId = 1,
                Username = "a"
            };

            _refreshTokenService.Setup(x => x.isValidRefreshToken(1, "token1")).Returns(true);
            _userRepo.Setup(x => x.GetUserInfo(1)).Returns(user);
            _profileService.Setup(x => x.GetUserProfile(1)).Returns(new UserProfile());
            _refreshTokenService.Setup(x => x.SaveRefreshToken(1, It.IsAny<string>(), It.IsAny<DateTime>())).Returns(true);

            var result = _service.GetNewAccessTokenIfRefreshTokenValid(1, "token1", "new token");

            Assert.AreEqual("200", result.status);
        }

        [TestMethod]
        public void RefreshToken_Invalid_Return401()
        {
            _refreshTokenService.Setup(x => x.isValidRefreshToken(1, "token1")).Returns(false);

            var result = _service.GetNewAccessTokenIfRefreshTokenValid(1, "token1", "method1");

            Assert.AreEqual("401", result.status);
        }

        [TestMethod]
        public void ResetPassword_Success()
        {
            _cache.Set("token123", new TempTokenData { UserId = 1, isVerified = true });
            _userRepo.Setup(x => x.ResetPassword(1, "newpass")).Returns(true);

            var result = _service.ResetPassword(1, "newpass", "newpass", "token123");

            Assert.AreEqual("200", result.status);
        }

        [TestMethod]
        public void ResetPassword_UnverifiedToken_Return401()
        {
            _cache.Set("token123", new TempTokenData { UserId = 1, isVerified = false });
            var result = _service.ResetPassword(1, "a", "a", "token123");

            Assert.AreEqual("401", result.status);
        }
    }
}
