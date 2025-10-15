using FishFarm.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FishFarmAPI_v2.Controllers
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("api/v1/sys")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DeviceService _deviceService;

        public UserController(IConfiguration configuration)
        {
            _userService = new UserService(configuration);
            _deviceService = new DeviceService();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = _userService.Login(request.Username, request.Password);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return Ok(result);

            } catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = e.Message });
            }
            
        }
    }
}
