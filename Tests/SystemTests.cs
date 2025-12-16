using System.Security.Claims;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
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
        private SystemController _controller = null!;
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

            //_controller = CreateControllerWithUser(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim ( "userId", "42" ) }, "Test")));

            Environment.SetEnvironmentVariable("Jwt__Key", "vT9kY3qL4FxP8mS1H0bN7cW2QpR5uJ6tZyA3KdF8MvB2TxQ1");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
            Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        }

        //private SystemController CreateControllerWithUser(ClaimsPrincipal? principal = null)
        //{
        //    var controller = new SystemController(_service);
        //    if (principal != null)
        //    {
        //        controller.ControllerContext = new ControllerContext
        //        {
        //            HttpContext = new DefaultHttpContext { User = principal}
        //        };
        //    }
        //    return controller;

        //}

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

        [TestMethod]
        public void ValidteTempToken_ReturnsOk_WhenTokenIsValid()
        {

            var tempTokenKey = "validateToken";
            var userToken = new TempTokenData
            {
                UserId = 1,
                DeviceId = "device123",
                isVerified = true,
                Purpose = "login",
                Method = "sms",
            };

            var userProfile = new UserProfile();
            var user = new User
            {
                UserId = 1,
                Phone = "1234567890",
                Email = "",
            };

            var device = new Device
            {
                UserId = 1,
                DeviceId = "device123",
                IsVerified = true,
            };

            _cache.Set(tempTokenKey, userToken);

            _deviceService.Setup(s => s.AddOrUpdateDeviceIsVerified(
                userToken.DeviceId,
                userToken.UserId,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(device);

            _profileService.Setup(s => s.GetUserProfile(userToken.UserId))
                .Returns(userProfile);

            _userRepo.Setup(s => s.GetUserInfo(userToken.UserId))
                .Returns(user);

            _refreshTokenService.Setup(s => s.SaveRefreshToken(
                userToken.UserId,
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(true);

            var result = _service.ValidateTempToken(tempTokenKey);

            Console.WriteLine(result.message);

            Assert.AreEqual("200", result.status);

        }

        [TestMethod]
        public void ValidteTempToken_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            var userToken = new TempTokenData
            {
                UserId = 1,
                DeviceId = "device123",
                isVerified = true,
                Purpose = "login",
                Method = "sms",
            };

            var userProfile = new UserProfile();
            var user = new User
            {
                UserId = 1,
                Phone = "1234567890",
                Email = "",
            };

            var device = new Device
            {
                UserId = 1,
                DeviceId = "device123",
                IsVerified = true,
            };

            _cache.Set("invalidTokenKey", userToken);

            _deviceService.Setup(s => s.AddOrUpdateDeviceIsVerified(
              userToken.DeviceId,
              userToken.UserId,
              It.IsAny<string>(),
              It.IsAny<string>()))
              .Returns(device);

            _profileService.Setup(s => s.GetUserProfile(userToken.UserId))
                .Returns(userProfile);

            _userRepo.Setup(s => s.GetUserInfo(userToken.UserId))
                .Returns(user);

            _refreshTokenService.Setup(s => s.SaveRefreshToken(
                userToken.UserId,
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(true);

            var result = _service.ValidateTempToken("key");

            Console.WriteLine(result.message);

            Assert.AreEqual("401", result.status);
        }


        [TestMethod]
        public void ValidateTempToken_ReturnsExpectedResult()
        {
            var userToken = new TempTokenData
            {
                UserId = 1,
                DeviceId = "device123",
                isVerified = true,
                Purpose = "notLogin",
                Method = "sms",
            };

            var userProfile = new UserProfile();
            var user = new User
            {
                UserId = 1,
                Phone = "1234567890",
                Email = "",
            };

            var device = new Device
            {
                UserId = 1,
                DeviceId = "device123",
                IsVerified = true,
            };

            _cache.Set("validTokenKey", userToken);

            _deviceService.Setup(s => s.AddOrUpdateDeviceIsVerified(
              userToken.DeviceId,
              userToken.UserId,
              It.IsAny<string>(),
              It.IsAny<string>()))
              .Returns(device);

            _profileService.Setup(s => s.GetUserProfile(userToken.UserId))
                .Returns(userProfile);

            _userRepo.Setup(s => s.GetUserInfo(userToken.UserId))
                .Returns(user);

            _refreshTokenService.Setup(s => s.SaveRefreshToken(
                userToken.UserId,
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(true);

            var result = _service.ValidateTempToken("validTokenKey");

            Console.WriteLine(result.message);

            Assert.AreEqual("401", result.status);
        }

        [TestMethod]
        public void ValidateGenericTempToken_ReturnsOk_WhenTokenIsValid()
        {
            string tempToken = "token";
            var userToken = new TempTokenData
            {
                UserId = 1,
                DeviceId = "device123",
                isVerified = false,
                Purpose = "generic",
                Method = "email",
            };

            _cache.Set(tempToken, userToken);

            var result = _service.ValidateGenericTempToken(tempToken);

            Console.WriteLine(result);

            var cachedToken = _cache.Get<TempTokenData>(tempToken);
            Assert.IsNotNull(cachedToken);
            Assert.IsTrue(cachedToken.isVerified);

            Assert.AreEqual(true, result);
        }

        //Controller-API Test
        [TestMethod]
        public void Login_SystemController_Ok()
        {
            var loginRequest = new FishFarm.BusinessObjects.LoginRequest
            {
                Username = "test",
                Password = "pass",
                DeviceId = "device1"
            };

            var result = new LoginResponse
            {
                status = "200",
                message = "Login successful",
                isDeviceVerified = true,
                accessToken = "a",
                refreshExpiresIn = 86400,
                expiresIn = 3600,
                refreshToken = "b",
                scope = "profile email",
                tokenType = "Bearer",
                userId = 1,
            };

            var userServiceMock = new Mock<IUserService>();
            
            userServiceMock.Setup(s => s.Login(loginRequest.Username, loginRequest.Password, loginRequest.DeviceId))
                .Returns(result);

            var controller = new SystemController(userServiceMock.Object);

            IActionResult actionResult = controller.Login(loginRequest);

            var oK = actionResult as OkObjectResult;
            Assert.IsNotNull(oK);

            var payload = oK.Value;

            Assert.IsNotNull(payload);

            Assert.AreEqual("200", payload.GetType().GetProperty("status")!.GetValue(payload));

        }

        [TestMethod]
        public void Login_SystemController_Cancel() 
        {
            var loginRequest = new FishFarm.BusinessObjects.LoginRequest
            {
                Username = "test",
                Password = "pass",
                DeviceId = "device1"
            };

            var result = new LoginResponse
            {
                status = "401",
                message = "",
                isDeviceVerified = true,
                accessToken = "a",
                refreshExpiresIn = 86400,
                expiresIn = 3600,
                refreshToken = "b",
                scope = "profile email",
                tokenType = "Bearer",
                userId = 1,
            };

            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(s => s.Login(loginRequest.Username, loginRequest.Password, loginRequest.DeviceId))
                .Returns(result);

            var controller = new SystemController(userServiceMock.Object);

            IActionResult actionResult = controller.Login(loginRequest);

            var unauthoized = actionResult as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthoized);

            var payload = unauthoized.Value;

            Assert.IsNotNull(payload);

            Assert.AreEqual(401, unauthoized.StatusCode);
        }

        [TestMethod]
        public void GetToken_SystemController_Ok()
        {
            TempTokenRequest tempTokenRequest = new TempTokenRequest
            {
                tempToken = "abbbb"
            };

            LoginResponse response = new LoginResponse
            {
                status = "200",
                message = "Login successful",
                isDeviceVerified = true,
                accessToken = "a",
                refreshExpiresIn = 86400,
                expiresIn = 3600,
                refreshToken = "b",
                scope = "profile email",
                tokenType = "Bearer",
                userId = 1,
            };

            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(s => s.ValidateTempToken(tempTokenRequest.tempToken))
                .Returns(response);

            var controller = new SystemController(userServiceMock.Object);

            IActionResult actionResult = controller.GetToken(tempTokenRequest);

            var oK = actionResult as OkObjectResult;
            Assert.IsNotNull(oK);

            var payload = oK.Value;

            Assert.IsNotNull(payload);

            Assert.AreEqual("200", payload.GetType().GetProperty("status")!.GetValue(payload));

        }
    }
}
