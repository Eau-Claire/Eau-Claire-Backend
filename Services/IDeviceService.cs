using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Services
{
    public interface IDeviceService
    {
        public Device AddDevice(string deviceId, int userId, string? deviceName, string? deviceType);
        public bool CheckDeviceStatus(string deviceId);

        public bool CheckDeviceIsVerified(string deviceId, int userId);

        public Device AddOrUpdateDeviceIsVerified(string deviceId, int userId, string? deviceName, string? deviceType);
    }
}
