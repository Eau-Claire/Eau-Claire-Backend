using System.ComponentModel.DataAnnotations;
using FishFarm.BusinessObjects;
using FishFarm.Services;
using FishFarmAPI_v2.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace OtpTests
{

    [TestClass]
    public class OtpTests
    {
        public IMemoryCache _cache = null!;
        public IOtpService _otpService = null!;
        public string otp = "";
        public Mock<IUtils> _utils = new Mock<IUtils>();

        [TestInitialize]
        public void Init()
        {
            Environment.SetEnvironmentVariable("SendGrid__SendGridPassword", "abc");

            _cache = new MemoryCache(new MemoryCacheOptions());
            _otpService = new OtpService(_cache, _utils.Object);

        }

        [TestMethod]
        public void SendOTPCode_ReturnOk_WithExistedOTP()
        {
            string email = "phamhoangminhchau1973@gmail.com";
            string device = "device1";
            string cacheKey = $"otp_{device}_{email}";

            _utils.Setup(s => s.GetEmailOtpTemplate(It.IsAny<string>()));

            var result = _otpService.SendOtp("email", device, "", email);

            Assert.IsNotNull(result);
            Assert.AreEqual("200", result.Result.ErrorCode);
            Assert.AreEqual(true, result.Result.IsSuccess);

            Console.WriteLine(_cache.Get<string>(cacheKey));

        }

        [TestMethod]
        public async Task SendOTPCode_Controller_ReturnOk_WithExistedOTP()
        {
            var request = new OtpRequest
            {
                Method = "email",
                DeviceId = "device1",
                Email = "test@gmail.com",
                Phone = "",
                Purpose = "login"
            };

            var serviceResult = new ServiceResult
            {
                ErrorCode = "200",
                IsSuccess = true,
                Message = "OTP already exists"
            };

            var otpServiceMock = new Mock<IOtpService>();
            otpServiceMock
                .Setup(s => s.SendOtp("email", "device1", "", "test@gmail.com"))
                .ReturnsAsync(serviceResult);

            var controller = new OtpController(otpServiceMock.Object);

            var result = await controller.GetOtpCode(request);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var payload = ok.Value as ServiceResult;
            Assert.IsNotNull(payload);
            Assert.AreEqual("200", payload.ErrorCode);
        }

        [TestMethod]
        public void VerifyOTPCode_Controller_ReturnUnauthorized_WhenOtpInvalid()
        {
            var request = new OtpRequest
            {
                Method = "email",
                InputOtp = "123456",
                Purpose = "login",
                Email = "test@gmail.com",
                DeviceId = "device1"
            };

            var verifyResult = new ServiceResult
            {
                IsSuccess = false
            };

            var otpServiceMock = new Mock<IOtpService>();
            otpServiceMock
                .Setup(s => s.VerifyOtp(
                    "email",
                    "123456",
                    It.IsAny<int?>(),
                    "device1",
                    null,
                    "test@gmail.com",
                    "login"))
                .Returns(verifyResult);

            var controller = new OtpController(otpServiceMock.Object);

            var result = controller.VerifyOtpCode(request);

            var unauthorized = result as ObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual(401, unauthorized.StatusCode);
        }

        [TestMethod]
        public void VerifyOTPCode_Success_Test()
        {
            SendOTPCode_ReturnOk_WithExistedOTP();

            string email = "phamhoangminhchau1973@gmail.com";
            string device = "device1";
            string cacheKey = $"otp_{device}_{email}";

            string otp = _cache.Get<string>(cacheKey)!;
            Console.WriteLine("Nay la:" + otp);

            var result = _otpService.VerifyOtp("email", otp, null, device, null, email, "test-purpose");

            Assert.IsNotNull(result.Data);
            Assert.AreEqual(true, result.IsSuccess);
            
            Console.WriteLine(result);
        }
    }
}
