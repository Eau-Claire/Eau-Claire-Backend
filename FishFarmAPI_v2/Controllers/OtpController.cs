using FishFarm.BusinessObjects;
using FishFarm.Services;
using Microsoft.AspNetCore.Mvc;

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
        [Route("request-otp")]
        public async Task<IActionResult> GetOtpCode([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceId))
            {
                return StatusCode(500, new { Message = "Device Id and User Id can not be empty" });
            }

            var otp = _otpService.GenerateOtp(6);

            if (otp == null)
            {
                return StatusCode(500, new { Message = "Failed to generate OTP" });
            }

            var result = await _otpService.SendOtp(request.Method, otp, request.DeviceId, request.Phone, request.Email);

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
        [Route("verify-otp")]
        public IActionResult VerifyOtpCode([FromBody] OtpRequest request)
        {

            if (string.IsNullOrEmpty(request.Method) 
                || string.IsNullOrEmpty(request.InputOtp))
            {
                return BadRequest(new { Message = "Method or InputOtp is missing" });
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
