using FishFarm.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FishFarmAPI_v2.Controllers
{
    public class DeviceRequest
    {
        public string DeviceId { get; set; }
        public int UserId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
    }

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
        public IActionResult GetDeviceInformation([FromBody] DeviceRequest deviceRequest)
        {
            try
            {
                var device = _deviceService.AddDevice(deviceRequest.DeviceId, deviceRequest.UserId, deviceRequest.DeviceName, deviceRequest.DeviceType);

                return Ok();
            } catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred while adding the device", error = e.Message });
            }
        }
    }
}
