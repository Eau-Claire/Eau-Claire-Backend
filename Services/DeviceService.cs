using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.Repositories;

namespace FishFarm.Services
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;
        
        public DeviceService()
        {
            _deviceRepository = new DeviceRepository();
        }

        public bool AddDevice(int userId, string deviceName, string deviceType)
        {
            return _deviceRepository.AddDevice(userId, deviceName, deviceType);
        }

        public bool CheckDeviceStatus(Guid deviceId)
        {
            return _deviceRepository.CheckDeviceStatus(deviceId);
        }
    }
}
