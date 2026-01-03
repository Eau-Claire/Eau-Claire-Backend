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

        public Device AddDevice(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {
            return _deviceRepository.AddDevice( userId, deviceIdentifier, deviceName, deviceType);
        }

        public Device AddOrUpdateDeviceIsVerified(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {
            return _deviceRepository.AddOrUpdateDeviceIsVerified(userId, deviceIdentifier, deviceName, deviceType);
        }

        public bool CheckDeviceIsVerified(string deviceIdentifier, int userId)
        {
            return _deviceRepository.CheckDeviceIsVerified(deviceIdentifier, userId);
        }

        public bool CheckDeviceStatus(string deviceIdentifier, int userId)
        {
            return _deviceRepository.CheckDeviceStatus(deviceIdentifier, userId);
        }
    }
}
