using FishFarm.BusinessObjects;
using FishFarm.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/sys")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;

        public UserController(IMemoryCache cache, IConfiguration configuration)
        {
            _userService = new UserService(cache, configuration);
            _deviceService = new DeviceService();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _userService.Login(request.Username, request.Password, request.DeviceId);

            if (result.status == "401")
            {
                return Unauthorized(new { message = result.message, isDeviceVerified = result.isDeviceVerified });
            } else if (result.status == "500")
            {
                return StatusCode(500, new { message = result.message, isDeviceVerified = result.isDeviceVerified });
            } else if (result == null)
            {
                return StatusCode(500, new { message = "An unknown error occurred during login" });
            }

            return Ok(result);
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] TempTokenRequest tempTokenRequest)
        {
            if (tempTokenRequest.Purpose == "login")
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
            else if (tempTokenRequest.Purpose == "register")
            {
                var registerResponse = _userService.ValidateRegistrationTempToken(tempTokenRequest.tempToken);
                if (registerResponse == null)
                {
                    return StatusCode(500, new { message = "An unknown error occurred during token validation" });
                }
                return Ok(registerResponse);
            }
            else if (tempTokenRequest.Purpose == "generic")
            {
                var resetResponse = _userService.ValidateGenericTempToken(tempTokenRequest.tempToken);
                if (resetResponse == false)
                {
                    return StatusCode(500, new { message = "An unknown error occurred during token validation" });
                }
                return Ok(new { status = "success", message = "Temp token is valid" });
            }
            else
            {
                return BadRequest(new { message = "Invalid purpose for temp token" });
            }
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

        [HttpGet("get-user-by-username")]
        public IActionResult GetUserByUsername([FromQuery] string username)
        {
            var user = _userService.GetUserInfoByUsername(username);
            if (user == null)
            {
                return NotFound(new {status = "", Message = "User not found" });
            }
            
            return Ok(new {status = "success", userId = user.UserId, username = user.Username, phone = user.Phone, email = user.Email});
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = _userService.ResetPassword(request.UserId, request.NewPassword, request.ConfirmPassword, request.TempToken);
            if (result.status == "500")
            {
                return StatusCode(500, new { Message = "Failed to reset password" });
            } else if (result.status == "400")
            {
                return BadRequest(new { Message = result.message });
            }

            return Ok(new { Message = "Password reset successfully" });
        }
    }
}
