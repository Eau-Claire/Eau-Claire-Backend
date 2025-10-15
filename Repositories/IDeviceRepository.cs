using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.Repositories
{
    public interface IDeviceRepository
    {
        public bool AddDevice(int userId, string deviceName, string deviceType);
        public bool CheckDeviceStatus(Guid deviceId);
    }
}
