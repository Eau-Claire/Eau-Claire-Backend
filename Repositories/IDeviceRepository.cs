using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Repositories
{
    public interface IDeviceRepository
    {
        public Device AddDevice(int userId, string deviceIdentifier, string? deviceName, string? deviceType);
        public bool CheckDeviceStatus(string deviceIdentifier, int userId);

        public bool CheckDeviceIsVerified(string deviceIdentifier, int userId);

        public Device AddOrUpdateDeviceIsVerified(int userId, string deviceIdentifier, string? deviceName, string? deviceType);
    }
}
