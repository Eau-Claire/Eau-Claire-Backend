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
                return Unauthorized(new {message = result.message, isDeviceVerified = result.isDeviceVerified});
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

            var loginResponse = _userService.ValidateTempToken(tempTokenRequest.tempToken);
            if (loginResponse.status == "401")
            {
                return Unauthorized(new {message = loginResponse.message, isDeviceVerified = loginResponse.isDeviceVerified});
            } else if (loginResponse.status == "500")
            {
                return StatusCode(500, new { message = loginResponse.message, isDeviceVerified = loginResponse.isDeviceVerified });
            }
            
            return Ok(loginResponse);
        }
    }
}
