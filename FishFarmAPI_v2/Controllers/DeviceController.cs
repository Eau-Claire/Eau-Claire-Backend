using FishFarm.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/sys")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public DeviceController()
        {
            _deviceService = new DeviceService();
        }

        [HttpPost("add-device")]
        public IActionResult GetDeviceInformation([FromBody] int userId, string deviceName, string deviceType)
        {
            try
            {
                bool isAdded = _deviceService.AddDevice(userId, deviceName, deviceType);

                return Ok();
            } catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred while adding the device", error = e.Message });
            }
        }
    }
}
