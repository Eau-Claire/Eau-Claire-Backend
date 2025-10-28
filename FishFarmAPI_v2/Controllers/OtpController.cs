using FishFarm.BusinessObjects;
using FishFarm.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FishFarmAPI_v2.Controllers
{

    [ApiController]
    [Route("api/v1/sys")]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;
        public OtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost]
        [EnableRateLimiting("otp")]
        [Route("request-otp")]
        public async Task<IActionResult> GetOtpCode([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceId))
            {
                return StatusCode(500, new { Message = "Device Id and User Id can not be empty" });
            }

            var result = await _otpService.SendOtp(request.Method, request.DeviceId, request.Phone, request.Email);

            if (result.ErrorCode == "500")
            {
                return StatusCode(500, result.Message);
            }
            else if (result.ErrorCode == "400")
            {
                return BadRequest(result.Message);
            }

            return Ok(result);

        }

        [HttpPost]
        [EnableRateLimiting("otp")]
        [Route("verify-otp")]
        public IActionResult VerifyOtpCode([FromBody] OtpRequest request)
        {

            if (string.IsNullOrEmpty(request.Method) 
                || string.IsNullOrEmpty(request.InputOtp) || string.IsNullOrEmpty(request.Purpose))
            {
                return BadRequest(new { Message = "Method, InputOtp or Purpose is missing" });
            }

            if (request.Method != "sms" && request.Method != "email")
            {
                return BadRequest(new { Message = "Invalid method. Use 'sms' or 'email'." });
            }
            else if (request.Method == "sms" && string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest(new { Message = "Phone number is required for SMS method" });
            }
            else if (request.Method == "email" && string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Email is required for Email method" });
            }

            var tempToken = _otpService.VerifyOtp(request.Method, request.InputOtp, request.UserId, request.DeviceId, request.Phone, request.Email, request.Purpose);

            if (string.IsNullOrEmpty(tempToken))
            {
                return StatusCode(401, new { Message = "Invalid or expired OTP" });
            }

            return Ok(new { tempToken });

        }
    }
}
