using System.ComponentModel.DataAnnotations;
using FishFarm.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace OtpTests
{

    [TestClass]
    public class OtpTests
    {
        public IMemoryCache _cache;
        public IOtpService _otpService;
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
        public void VerifyOTPCode_Success_Test()
        {
            SendOTPCode_ReturnOk_WithExistedOTP();

            string email = "phamhoangminhchau1973@gmail.com";
            string device = "device1";
            string cacheKey = $"otp_{device}_{email}";

            string otp = _cache.Get<string>(cacheKey);
            Console.WriteLine("Nay la:" + otp);

            var result = _otpService.VerifyOtp("email", otp, null, device, null, email, "test-purpose");

            Assert.IsNotNull(result.Data);
            Assert.AreEqual(true, result.IsSuccess);
            
            Console.WriteLine(result);
        }
    }
}
