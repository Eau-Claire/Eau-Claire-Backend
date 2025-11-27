using System.Security.Claims;
using FishFarm.BusinessObjects;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UserTests
{
    [TestClass]
    public sealed class UserTests
    {
        public Mock<IUserService> _userService = null!;

        [TestInitialize]
        public void Init ()
        {
            _userService = new Mock<IUserService>();
        }

        // Helper: tạo controller với optional ClaimsPrincipal
        private UserController CreateControllerWithUser(ClaimsPrincipal? principal = null)
        {
            var controller = new UserController(_userService.Object);
            if (principal != null)
            {
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = principal }
                };
            }
            return controller;
        }

        [TestMethod]
        public void GetUserByUsername_ReturnsOk_WithCreateUserResponse_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Phone = null,
                Email = "testuser@gmail.com"
            };
            _userService.Setup(s => s.GetUserInfoByUsername("testuser")).Returns(user);

            var controller = CreateControllerWithUser();

            // Act
            IActionResult actionResult = controller.GetUserByUsername("testuser");

            // Assert: unwrap OkObjectResult -> CreateUserResponse
            var ok = actionResult as OkObjectResult;
            Assert.IsNotNull(ok, "Expected OkObjectResult");
            var resp = ok!.Value as CreateUserResponse;
            Assert.IsNotNull(resp, "Expected CreateUserResponse inside OkObjectResult");

            Assert.AreEqual("Success", resp.Status); // note: your controller uses "Success" here
            Assert.AreEqual("User Found", resp.Message);
            Assert.AreEqual(user.UserId, resp.UserId);
            Assert.AreEqual(user.Username, resp.Username);
            Assert.AreEqual("", resp.Phone); // controller maps null -> ""
            Assert.AreEqual(user.Email, resp.Email);
        }

        [TestMethod]
        public void GetUserByUsername_ReturnsNotFound_WhenUserMissing()
        {
            // Arrange
            _userService.Setup(s => s.GetUserInfoByUsername("missing")).Returns((User?)null);

            var controller = CreateControllerWithUser();

            // Act
            IActionResult actionResult = controller.GetUserByUsername("missing");

            // Assert
            var notFound = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFound, "Expected NotFoundObjectResult");
        }

        [TestMethod]
        public void GetUserInfo_ReturnsOk_WhenUserExists_FromClaims()
        {
            // Arrange
            var user = new User
            {
                UserId = 42,
                Username = "claimuser",
                Phone = "012345",
                Email = "a@b.com"
            };
            _userService.Setup(s => s.GetUserInfo(42)).Returns(user);

            // prepare principal with claim "userId"
            var claims = new[] { new Claim("userId", "42") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var controller = CreateControllerWithUser(principal);

            // Act
            IActionResult actionResult = controller.GetUserInfo();

            // Assert
            var ok = actionResult as OkObjectResult;
            Assert.IsNotNull(ok);
            var resp = ok!.Value as CreateUserResponse;
            Assert.IsNotNull(resp);
            Assert.AreEqual("success", resp.Status); // note: controller uses lowercase "success" here
            Assert.AreEqual(user.UserId, resp.UserId);
            Assert.AreEqual(user.Username, resp.Username);
            Assert.AreEqual(user.Phone, resp.Phone);
            Assert.AreEqual(user.Email, resp.Email);
        }

        [TestMethod]
        public void GetUserInfo_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            _userService.Setup(s => s.GetUserInfo(It.IsAny<int>())).Returns((User?)null);

            var claims = new[] { new Claim("userId", "99") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
            var controller = CreateControllerWithUser(principal);

            // Act
            IActionResult actionResult = controller.GetUserInfo();

            // Assert
            var notFound = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
            // payload is an anonymous object in your controller (new { status = "", Message = "User not found" })
            // so simply assert it's not null and contains expected message:
            var payload = notFound!.Value;
            Assert.IsNotNull(payload);
            // You can use reflection to check Message property:
            var messageProp = payload.GetType().GetProperty("Message");
            Assert.IsNotNull(messageProp);
            Assert.AreEqual("User not found", messageProp!.GetValue(payload));
        }
    }
}

