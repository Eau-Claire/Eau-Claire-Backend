using FishFarm.BusinessObjects;
using FishFarm.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/sys")]
    public class SystemController : Controller
    {
        private readonly UserService _userService;
        public SystemController(IMemoryCache cache, IConfiguration configuration)
        {
            _userService = new UserService(cache, configuration);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _userService.Login(request.Username, request.Password, request.DeviceId);

            if (result == null)
            {
                return StatusCode(500, new { message = "An unknown error occurred during login" });
            }

            else if (result.status == "401")
            {
                return Unauthorized(new { message = result.message, isDeviceVerified = result.isDeviceVerified });
            }
            else if (result.status == "500")
            {
                return StatusCode(500, new { message = result.message, isDeviceVerified = result.isDeviceVerified });
            }


            return Ok(result);
        }

        [HttpPost("auth/token")]
        public IActionResult GetToken([FromBody] TempTokenRequest tempTokenRequest)
        {

            var loginResponse = _userService.ValidateTempToken(tempTokenRequest.tempToken);
            if (loginResponse == null)
            {
                return StatusCode(500, new { message = "An unknown error occurred during token validation" });
            }

            if (loginResponse.status == "401")
            {
                return Unauthorized(new { message = loginResponse.message, isDeviceVerified = loginResponse.isDeviceVerified });
            }
            else if (loginResponse.status == "500")
            {
                return StatusCode(500, new { message = loginResponse.message, isDeviceVerified = loginResponse.isDeviceVerified });
            }

            return Ok(loginResponse);
        }

        [HttpPost("token")]
        public IActionResult GetTokenForGeneric([FromBody] TempTokenRequest tempTokenRequest)
        {
            var resetResponse = _userService.ValidateGenericTempToken(tempTokenRequest.tempToken);
            if (resetResponse == false)
            {
                return StatusCode(500, new { message = "An unknown error occurred during token validation" });
            }
            return Ok(new { status = "success", message = "Temp token is valid" });

        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = _userService.GetNewAccessTokenIfRefreshTokenValid(request.UserId, request.RefreshToken);
            if (result == null)
            {
                return StatusCode(500, new { message = "An unknown error occurred during token refresh" });
            }
            else if (result.status == "401")
            {
                return Unauthorized(new { message = result.message });
            }
            else if (result.status == "500")
            {
                return StatusCode(500, new { message = result.message });
            }
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = _userService.ResetPassword(request.UserId, request.NewPassword, request.ConfirmPassword, request.TempToken);
            if (result.status == "500")
            {
                return StatusCode(500, new { Message = "Failed to reset password" });
            }
            else if (result.status == "400")
            {
                return BadRequest(new { Message = result.message });
            }

            return Ok(new { Message = "Password reset successfully" });
        }
    }
}

