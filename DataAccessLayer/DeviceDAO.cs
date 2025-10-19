using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FishFarm.DataAccessLayer
{
    public class DeviceDAO
    {
        private FishFarmDbV2Context _context = new FishFarmDbV2Context();

        public Device AddDevice(string deviceId, int userId, string? deviceName, string? deviceType)
        {

            Device device = new Device 
            { 
                UserId = userId, 
                DeviceName = deviceName ?? "", 
                DeviceType = deviceType ?? "", 
                DeviceId = deviceId!, 
                CreatedAt = DateTime.UtcNow, 
                ExpiredAt = DateTime.UtcNow.AddDays(1),
                IsVerified = true
            };

            _context.Device.Add(device);
            _context.SaveChanges();

            return device;
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

        public bool CheckDeviceIsVerified (string deviceId, int userId)
        {
            Device existedDevice = _context.Device.FirstOrDefault(d => d.DeviceId == deviceId && d.UserId == userId)!;

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
        
        public Device AddOrUpdateDeviceIsVerified(string deviceId, int userId, string? deviceName, string? deviceType)
        {
            try
            {
                Device existedDevice = _context.Device.FirstOrDefault(d => d.DeviceId == deviceId && d.UserId == userId)!;
                if (existedDevice == null)
                {
                    var addedDevice = AddDevice(deviceId, userId, deviceName, deviceType);
                    return addedDevice;
                }
                existedDevice.IsVerified = true;
                _context.Device.Update(existedDevice);
                _context.SaveChanges();
                return existedDevice;

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DbUpdateException: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return null;
            }
            
        }
    }
}
