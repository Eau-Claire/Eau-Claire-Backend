using FishFarm.BusinessObjects;
using FishFarm.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(IMemoryCache cache, IConfiguration configuration)
        {
            _userService = new UserService(cache, configuration);
        }

        [HttpGet("get-user-by-username")]
        public IActionResult GetUserByUsername([FromQuery] string username)
        {
            var user = _userService.GetUserInfoByUsername(username);
            if (user == null)
            {
                return NotFound(new { status = "", Message = "User not found" });
            }

            return Ok(new { status = "success", userId = user.UserId, username = user.Username, phone = user.Phone, email = user.Email });
        }

        [HttpGet("get-user-info")]
        public IActionResult GetUserInfo()
        {
            int userId = User.FindFirst("userId") != null ? int.Parse(User.FindFirst("userId")!.Value) : 0;
            var user = _userService.GetUserInfo(userId);
            if (user == null)
            {
                return NotFound(new { status = "", Message = "User not found" });
            }
            return Ok(new { status = "success", user = user });
        }

    }
}
