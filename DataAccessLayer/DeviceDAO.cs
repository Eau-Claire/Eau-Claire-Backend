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
        private readonly FishFarmContext _context;
        public DeviceDAO(FishFarmContext context)
        {
            _context = context;
        }

        public Device AddDevice(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {

            Device device = new Device 
            { 
                UserId = userId, 
                DeviceName = deviceName ?? "", 
                DeviceIdentifier = deviceIdentifier ?? "",
                DeviceType = deviceType ?? "", 
                CreatedAt = DateTime.UtcNow, 
                ExpiredAt = DateTime.UtcNow.AddDays(1),
                IsVerified = true
            };

            _context.Devices.Add(device);
            _context.SaveChanges();

            return device;
        }

        public bool CheckDeviceStatus(string deviceIdentifier, int userId)
        {
            try
            {
                var device = _context.Devices.FirstOrDefault(d => d.DeviceIdentifier == deviceIdentifier && d.UserId == userId)!;
                DateTime createdAt = device.CreatedAt;
                DateTime expiredAt = device.ExpiredAt;

                TimeSpan timeSpan = (expiredAt - createdAt);

                if (timeSpan < TimeSpan.Zero)
                {
                    _context.Devices.Remove(device);
                    return false;
                }
                return true;

            }catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        public bool CheckDeviceIsVerified (string deviceIdentifier, int userId)
        {
            Device existedDevice = _context.Devices.FirstOrDefault(d => d.DeviceIdentifier == deviceIdentifier && d.UserId == userId)!;

            if (existedDevice == null) 
            {
                return false;
            }

            if (existedDevice.IsVerified) 
            {
                return true;
            } else
            {
                return false;
            }
        }
        
        public Device AddOrUpdateDeviceIsVerified(int userId, string deviceIdentifier, string? deviceName, string? deviceType)
        {
            try
            {
                Device existedDevice = _context.Devices.FirstOrDefault(d => d.DeviceIdentifier == deviceIdentifier && d.UserId == userId)!;
                if (existedDevice == null)
                {
                    var addedDevice = AddDevice(userId, deviceIdentifier, deviceName, deviceType);
                    return addedDevice;
                }
                existedDevice.IsVerified = true;
                _context.Devices.Update(existedDevice);
                _context.SaveChanges();
                return existedDevice;

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DbUpdateException: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                return new Device();
            }
            
        }
    }
}
