using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.DataAccessLayer;

namespace FishFarm.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DeviceDAO _deviceDAO;

        public DeviceRepository()
        {
            _deviceDAO = new DeviceDAO();
        }

        public Device AddDevice(string deviceId, int userId, string? deviceName, string? deviceType)
        {
            return _deviceDAO.AddDevice(deviceId, userId, deviceName, deviceType);
        }

        public Device AddOrUpdateDeviceIsVerified(string deviceId, int userId, string? deviceName, string? deviceType)
        {
            return _deviceDAO.AddOrUpdateDeviceIsVerified(deviceId, userId, deviceName, deviceType);
        }

        public bool CheckDeviceIsVerified(string deviceId, int userId)
        {
            return _deviceDAO.CheckDeviceIsVerified(deviceId, userId);
        }

        public bool CheckDeviceStatus(string deviceId)
        {
            return _deviceDAO.CheckDeviceStatus(deviceId);
        }
    }
}
