using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.DataAccessLayer
{
    public class DeviceDAO
    {
        private FishFarmDbV2Context _context = new FishFarmDbV2Context();

        public bool AddDevice(int userId, string deviceName, string deviceType)
        {

            Device device = new Device 
            { 
                UserId = userId, 
                DeviceName = deviceName, 
                DeviceType = deviceType, 
                Token = Guid.NewGuid().ToString(), 
                CreatedAt = DateTime.UtcNow, 
                ExpiredAt = DateTime.UtcNow.AddDays(1),
                IsActive = true 
            };

            _context.Device.Add(device);
            _context.SaveChanges();

            return true;
        }

        public bool CheckDeviceStatus(Guid deviceId)
        {
            var device = _context.Device.FirstOrDefault(d => d.Id == deviceId);
            DateTime createdAt = device.CreatedAt;
            DateTime expiredAt = device.ExpiredAt;

            TimeSpan timeSpan = (expiredAt - createdAt);

            if (timeSpan < TimeSpan.Zero)
            {
                _context.Device.Remove(device);
                return false;
            }
            return true;
        }
    }
}
