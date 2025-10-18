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
        public IActionResult GetOtpCode([FromBody] OtpRequest request)
        {
            try
            {
                var otp = _otpService.GenerateOtp(6);

                if (otp == null) {
                    return StatusCode(500, new { Message = "Failed to generate OTP" });
                }

                var result = _otpService.SendOtp(request.Method, otp, request.UserId, request.DeviceId, request.Phone, request.Email);
                
                if (result.ErrorCode == "500")
                {
                    return StatusCode(500, result.Message);
                } else if (result.ErrorCode == "409")
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error generating or sending OTP", Details = ex.Message });
            }
        }

        [HttpPost]
        [Route("verify-otp")]
        public IActionResult VerifyOtpCode([FromBody] OtpRequest request)
        {
            try
            {
                if (request.Method == null || request.InputOtp == null)
                {
                    return BadRequest(new { Message = "Method or InputOtp is missing" });
                }

                var tempToken = _otpService.VerifyOtp(request.Method, request.InputOtp, request.UserId, request.DeviceId, request.Phone, request.Email);

                if (tempToken == "")
                {
                    return StatusCode(401, new { Message = "Invalid or expired OTP" });
                }

                return Ok(new {tempToken});

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error verifying OTP", Details = ex.Message });

            }
        }
    }
}
