using FishFarm.BusinessObjects;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests
{
    [TestClass]
    public sealed class UserTests
    {

        [TestMethod]
        public void GetUserByUsername_Found_ReturnOkWithExpectedPayload()
        {
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Phone = "",
                Email = "testuser@gmail.com"
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetUserInfoByUsername("testuser")).Returns(user);

            var controller = new UserController(mockUserService.Object);

            var result = controller.GetUserByUsername("testuser");

            var okResult = result as OkObjectResult;

            var response = okResult!.Value as CreateUserResponse;

            Assert.AreEqual("success", response!.Status);
            Assert.AreEqual("testuser", response.Username);
            Assert.AreEqual("", response.Phone);
            Assert.AreEqual("testuser@gmail.com", response.Email);
        }

        [TestMethod]
        public void GetUserByUsername_NotFound_ReturnNotFound()
        {

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetUserInfoByUsername("testuser"));

            var controller = new UserController(mockUserService.Object);

            var result = controller.GetUserByUsername("testuser");

            var okResult = result as NotFoundObjectResult;

            var response = okResult!.Value as CreateUserResponse;

            Assert.AreEqual("Not Found", response!.Status);
        }
    }

}

