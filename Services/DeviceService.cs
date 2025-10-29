using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;

namespace FishFarm.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        
        public DeviceService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public Device AddDevice(string deviceId, int userId, string? deviceName, string? deviceType)
        {
            return _deviceRepository.AddDevice(deviceId, userId, deviceName, deviceType);
        }

        public Device AddOrUpdateDeviceIsVerified(string deviceId, int userId, string? deviceName, string? deviceType)
        {
            return _deviceRepository.AddOrUpdateDeviceIsVerified(deviceId, userId, deviceName, deviceType);
        }

        public bool CheckDeviceIsVerified(string deviceId, int userId)
        {
            return _deviceRepository.CheckDeviceIsVerified(deviceId, userId);
        }

        public bool CheckDeviceStatus(string deviceId)
        {
            return _deviceRepository.CheckDeviceStatus(deviceId);
        }
    }
}
