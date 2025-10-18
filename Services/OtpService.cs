using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Api.V2010.Account;

namespace FishFarm.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        private readonly TemplateService _templateService;
        private readonly DeviceService deviceService;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;


        public OtpService(IMemoryCache cache, IConfiguration config)
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            _fromPhoneNumber = config["Twilio:FromPhoneNumber"] ?? "";

            _cache = cache;
            _templateService = new TemplateService();
            deviceService = new DeviceService();
        }
        public string GenerateOtp(int length = 6)
        {
            const string digits = "0123456789";
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = digits[random.Next(digits.Length)];
            }

            return new string(otp);
        }

        public string FindExistedOtp(string method, int userId, string deviceId, string? phone, string? email)
        {
            var cacheKey = method == "sms" ? $"{userId}_otp_{deviceId}_{phone}" : $"{userId}_otp_{deviceId}_{phone}";
            return _cache.Get<string>(cacheKey) ?? "";
        }

        public ServiceResult SendOtp(string method, string otp, int userId, string deviceId, string? phone, string? email)
        {
            try
            {
                var cacheKey = string.Empty;
                var cacheOptions = new MemoryCacheEntryOptions();

                string existedOtp = FindExistedOtp(method,userId, deviceId, phone, email);

                if (existedOtp != null && existedOtp != "")
                {
                    _cache.Remove(cacheKey);
                }

                if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(otp))
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        ErrorCode = "409",
                        Message = "Method or OTP is missing",
                        Data = null
                    };
                }

                if (method == "email" && string.IsNullOrEmpty(email))
                {
                    return new ServiceResult 
                    {
                        IsSuccess = false,
                        ErrorCode = "409",
                        Message = "Email must be provided for Email method",
                        Data = null
                    };
                }

                if (method == "sms" && string.IsNullOrEmpty(phone))
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        ErrorCode = "409",
                        Message = "Phone number must be provided for SMS method",
                        Data = null
                    };
                }

                if (method == "sms")
                {
                    cacheKey = $"{userId}_otp_{deviceId}_{phone}";
                    cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)

                    };

                    _cache.Set(cacheKey, otp, cacheOptions);

                    Twilio.TwilioClient.Init(_accountSid, _authToken);
                    var message = MessageResource.Create(
                        body: $"Your OTP code is: {otp}",
                        from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(phone)
                    );

                    return new ServiceResult
                    {
                        IsSuccess = true,
                        ErrorCode = "200",
                        Message = "OTP sent successfully via SMS",
                        Data = null
                    };

                }

                cacheKey = $"{userId}_otp_{deviceId}_{email}";
                cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                _cache.Set(cacheKey, otp, cacheOptions);

                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("eauclaire1510@gmail.com", "rgyg xwmf giwk sdpb\r\n");
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("eauclaire1510@gmail.com", "Eau Claire Support"),
                        To = { new MailAddress(email ?? "eauclaire1510@gmail.com") },
                        Subject = "Your OTP Code to Verify Eau Claire account!",
                        Body = _templateService.GetEmailOtpTemplate(otp),
                        IsBodyHtml = true,

                    };

                    client.Send(mailMessage);

                    return new ServiceResult
                    {
                        IsSuccess = true,
                        ErrorCode = "200",
                        Message = "OTP sent successfully via Email",
                        Data = null

                    };
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP: {ex.Message}");
                return new ServiceResult 
                {
                    IsSuccess = false,
                    ErrorCode = "500",
                    Message = $"Error sending OTP: {ex.Message}",
                    Data = null

                };

            }
        }

        public string VerifyOtp(string method, string inputOtp, int userId, string deviceId, string? phone, string? email)
        {
            var cacheKey = method == "sms" ? $"{userId}_otp_{deviceId}_{phone}" : $"{userId}_otp_{deviceId}_{email}";

            var storedOtp = _cache.Get<string>(cacheKey);

            if (storedOtp != null && storedOtp == inputOtp)
            {
                _cache.Remove(cacheKey);
                var tempToken = Guid.NewGuid().ToString();

                _cache.Set(tempToken, new 
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Phone = phone,
                    Email = email,
                    Method = method

                }, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

                return tempToken;
            }
            return "";
        }
    }
}
