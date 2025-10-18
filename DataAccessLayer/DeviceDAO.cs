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
                DeviceId = Guid.NewGuid().ToString(), 
                CreatedAt = DateTime.UtcNow, 
                ExpiredAt = DateTime.UtcNow.AddDays(1),
                IsVerified = false 
            };

            _context.Device.Add(device);
            _context.SaveChanges();

            return true;
        }

        public bool CheckDeviceStatus(string deviceId)
        {
            var device = _context.Device.FirstOrDefault(d => d.DeviceId == deviceId);
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

        public bool CheckDeviceIsVerified (string deviceId, string userId)
        {
            Device existedDevice = _context.Device.FirstOrDefault(d => d.DeviceId == deviceId && d.UserId.ToString() == userId)!;

            if (existedDevice == null) 
            {
                return false;
            }

            if (existedDevice.IsVerified == true) 
            {
                return true;
            } else
            {
                return false;
            }
        } 
    }
}
