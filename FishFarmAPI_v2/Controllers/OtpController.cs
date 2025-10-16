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

                bool isSent = _otpService.SendOtp(request.Method, otp, request.Phone, request.Email);
                
                return Ok(new { IsOtpSent = isSent});

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error generating or sending OTP", Details = ex.Message });
            }
        }

        [HttpPost]
        [Route("verify-otp")]
        public IActionResult VerifyOtpCode([FromBody] OtpRequest otpRequest)
        {
            try
            {
                bool isVerified = _otpService.VerifyOtp(otpRequest.Method, otpRequest.InputOtp, otpRequest.Phone, otpRequest.Email);

                return Ok(new { Verified = isVerified });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error verifying OTP", Details = ex.Message });

            }
        }
    }
}
