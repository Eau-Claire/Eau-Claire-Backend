using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Services
{
    public interface IOtpService
    {
        public Task<ServiceResult> SendOtp(string method, string deviceId, string? phone, string? email);
        public ServiceResult VerifyOtp(string method, string inputOtp, int? userId, string deviceId, string? phone, string? email, string purpose);
    }
}
