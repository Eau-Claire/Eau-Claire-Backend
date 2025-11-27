using FishFarm.BusinessObjects;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SystemTests
{

    [TestClass]
    public class SystemTests
    {
        private Mock<IUserService> _userService = null!;

        [TestInitialize]
        public void Init()
        {
            _userService = new Mock<IUserService>();
        }

        private SystemController CreateController()
        {
            return new SystemController(_userService.Object);
        }


        // LOGIN SUCCESS
        [TestMethod]
        public void Login_Success_ReturnOk()
        {
            var req = new LoginRequest { Username = "a", Password = "b", DeviceId = "d1" };

            var expected = new LoginResponse
            {
                status = "200",
                message = "ok",
                isDeviceVerified = true
            };

            _userService
                .Setup(s => s.Login(req.Username, req.Password, req.DeviceId))
                .Returns(expected);

            var controller = CreateController();

            var result = controller.Login(req);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var obj = ok.Value as LoginResponse;
            Assert.AreEqual("200", obj!.status);
        }


        // LOGIN 401
        [TestMethod]
        public void Login_Unauthorized_Return401()
        {
            var req = new LoginRequest { Username = "a", Password = "b", DeviceId = "d1" };

            var expected = new LoginResponse
            {
                status = "401",
                message = "wrong",
                isDeviceVerified = false
            };

            _userService.Setup(s => s.Login(req.Username, req.Password, req.DeviceId)).Returns(expected);

            var result = CreateController().Login(req);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }


        // LOGIN ERROR 500
        [TestMethod]
        public void Login_ServerError_Return500()
        {
            var req = new LoginRequest { Username = "a", Password = "b", DeviceId = "d1" };

            var expected = new LoginResponse
            {
                status = "500",
                message = "error"
            };

            _userService.Setup(s => s.Login(req.Username, req.Password, req.DeviceId)).Returns(expected);

            var result = CreateController().Login(req);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = result as ObjectResult;
            Assert.AreEqual(500, obj!.StatusCode);
        }


        // LOGIN RESULT NULL
        [TestMethod]
        public void Login_ReturnNull_Return500()
        {
            var req = new LoginRequest();

            _userService.Setup(s => s.Login(req.Username, req.Password, req.DeviceId))
                        .Returns((LoginResponse?)null);

            var result = CreateController().Login(req);

            var obj = result as ObjectResult;
            Assert.AreEqual(500, obj!.StatusCode);
        }



        // VALIDATE TEMP TOKEN SUCCESS
        [TestMethod]
        public void ValidateTempToken_Success_ReturnOk()
        {
            var req = new TempTokenRequest { tempToken = "123" };

            var expected = new LoginResponse { status = "200", message = "ok" };

            _userService.Setup(s => s.ValidateTempToken("123"))
                        .Returns(expected);

            var res = CreateController().GetToken(req) as OkObjectResult;

            Assert.IsNotNull(res);
            Assert.AreEqual("200", ((LoginResponse)res!.Value!).status);
        }


        // VALIDATE TEMP TOKEN 401
        [TestMethod]
        public void ValidateTempToken_401_ReturnUnauthorized()
        {
            var req = new TempTokenRequest { tempToken = "123" };

            var expected = new LoginResponse { status = "401", message = "unauth" };

            _userService.Setup(s => s.ValidateTempToken("123")).Returns(expected);

            var res = CreateController().GetToken(req);

            Assert.IsInstanceOfType(res, typeof(UnauthorizedObjectResult));
        }


        // VALIDATE TEMP TOKEN NULL
        [TestMethod]
        public void ValidateTempToken_Null_Return500()
        {
            var req = new TempTokenRequest { tempToken = "123" };

            _userService.Setup(s => s.ValidateTempToken("123"))
                        .Returns((LoginResponse?)null);

            var res = CreateController().GetToken(req);

            var obj = res as ObjectResult;
            Assert.AreEqual(500, obj!.StatusCode);
        }


        // GENERIC TEMP TOKEN SUCCESS
        [TestMethod]
        public void ValidateGenericTempToken_Success()
        {
            var req = new TempTokenRequest { tempToken = "aaa" };

            _userService.Setup(s => s.ValidateGenericTempToken("aaa"))
                        .Returns(true);

            var res = CreateController().GetTokenForGeneric(req) as OkObjectResult;

            Assert.IsNotNull(res);
        }

        // GENERIC TEMP TOKEN FAIL
        [TestMethod]
        public void ValidateGenericTempToken_Fail_Return500()
        {
            var req = new TempTokenRequest { tempToken = "aaa" };

            _userService.Setup(s => s.ValidateGenericTempToken("aaa"))
                        .Returns(false);

            var res = CreateController().GetTokenForGeneric(req);
            var obj = res as ObjectResult;
            Assert.AreEqual(500, obj!.StatusCode);
        }


        // REFRESH TOKEN SUCCESS
        [TestMethod]
        public void RefreshToken_Success()
        {
            var req = new RefreshTokenRequest { UserId = 1, RefreshToken = "rt" };

            var expected = new LoginResponse { status = "200" };

            _userService.Setup(s => s.GetNewAccessTokenIfRefreshTokenValid(1, "rt", "new access token"))
                        .Returns(expected);

            var res = CreateController().RefreshToken(req) as OkObjectResult;

            Assert.IsNotNull(res);
        }

        // REFRESH TOKEN 401
        [TestMethod]
        public void RefreshToken_401()
        {
            var req = new RefreshTokenRequest { UserId = 1, RefreshToken = "rt" };

            _userService.Setup(s => s.GetNewAccessTokenIfRefreshTokenValid(1, "rt", "new access token"))
                        .Returns(new LoginResponse { status = "401" });

            var result = CreateController().RefreshToken(req);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        // RESET PASSWORD SUCCESS
        [TestMethod]
        public void ResetPassword_Success()
        {
            var req = new ResetPasswordRequest { UserId = 1, NewPassword = "a", ConfirmPassword = "a", TempToken = "t" };

            _userService.Setup(s => s.ResetPassword(1, "a", "a", "t"))
                        .Returns(new LoginResponse { status = "200" });

            var result = CreateController().ResetPassword(req) as OkObjectResult;

            Assert.IsNotNull(result);
        }


        // RESET PASSWORD 500
        //[TestMethod]
        //public void ResetPassword_500()
        //{
        //    var req = new ResetPasswordRequest { UserId = 1, NewPassword = "a", ConfirmPassword = "a", TempToken = "t" };

        //    _userService.Setup(s => s.ResetPassword(1, "a", "a", "t"))
        //                .Returns( new LoginResponse{ status = "500" });

        //    var result = CreateController().ResetPassword(req);

        //    var obj = result as ObjectResult;
        //    Assert.AreEqual(500, obj!.StatusCode);
        //}
    }
}
