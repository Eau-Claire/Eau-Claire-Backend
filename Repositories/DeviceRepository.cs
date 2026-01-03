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

        public DeviceRepository(DeviceDAO deviceDAO)
        {
            _deviceDAO = deviceDAO;
        }

        public Device AddDevice(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {
            return _deviceDAO.AddDevice(userId, deviceIdentifier, deviceName, deviceType);
        }

        public Device AddOrUpdateDeviceIsVerified(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {
            return _deviceDAO.AddOrUpdateDeviceIsVerified(userId, deviceIdentifier, deviceName, deviceType);
        }

        public bool CheckDeviceIsVerified(string deviceIdentifier, int userId)
        {
            return _deviceDAO.CheckDeviceIsVerified(deviceIdentifier, userId);
        }

        public bool CheckDeviceStatus(string deviceIdentifier, int userId)
        {
            return _deviceDAO.CheckDeviceStatus(deviceIdentifier, userId);
        }
    }
}
