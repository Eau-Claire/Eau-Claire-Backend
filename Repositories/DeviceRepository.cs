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

        public bool AddDevice(int userId, string deviceName, string deviceType)
        {
            return _deviceDAO.AddDevice(userId, deviceName, deviceType);
        }

        public bool CheckDeviceIsVerified(string deviceId, string userId)
        {
            return _deviceDAO.CheckDeviceIsVerified(deviceId, userId);
        }

        public bool CheckDeviceStatus(string deviceId)
        {
            return _deviceDAO.CheckDeviceStatus(deviceId);
        }
    }
}
