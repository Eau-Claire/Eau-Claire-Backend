using FishFarm.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FishFarmAPI_v2.Controllers
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string DeviceId { get; set; }
    }

    public class TempToken
    {
        public string tempToken { get; set; }
    }

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
            try
            {
                var result = _userService.Login(request.Username, request.Password, request.DeviceId);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                } else if (result.status == "Device not verified")
                {
                    return StatusCode(403, result);
                }

                    return Ok(result);

            } catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = e.Message });
            }
            
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] TempToken tempToken)
        {
            try
            {
                var loginResponse = _userService.ValidateTempToken(tempToken.tempToken);
                if (loginResponse.status != "success")
                {
                    return Unauthorized(loginResponse);
                }
                return Ok(loginResponse);
            } catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred during token validation", error = e.Message });
            }
        }
    }
}
