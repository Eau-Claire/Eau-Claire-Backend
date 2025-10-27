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
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        private readonly string _gmailAppPassword;

        private readonly DeviceService _deviceService;


        public OtpService(IMemoryCache cache, IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"] ?? "";
            _authToken = config["Twilio:AuthToken"] ?? "";
            _fromPhoneNumber = config["Twilio:FromPhoneNumber"] ?? "";

            _gmailAppPassword = config["Gmail:GmailAppPassword"] ?? "";

            _cache = cache;
            _templateService = new TemplateService();
            _deviceService = new DeviceService();
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


        public async Task<ServiceResult> SendOtp(string method, string otp, string deviceId, string? phone, string? email)
        {
            try
            {
                var cacheKey = method == "sms" ? $"otp_{deviceId}_{phone}" : $"otp_{deviceId}_{email}";
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };


                if (_cache.TryGetValue(cacheKey, out _))
                {
                    _cache.Remove(cacheKey);
                }

                if (method == "sms")
                {
                    Console.WriteLine("hay: " + _accountSid + " " + _authToken + "" + _fromPhoneNumber);

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
                else if (method == "email")
                {
                   
                    _cache.Set(cacheKey, otp, cacheOptions);

                    using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                    {
                        client.Port = 587;
                        client.Credentials = new NetworkCredential("eauclaire1510@gmail.com", _gmailAppPassword);
                        client.EnableSsl = true;

                        MailMessage mailMessage = new MailMessage
                        {
                            From = new MailAddress("eauclaire1510@gmail.com", "Eau Claire Support"),
                            To = { new MailAddress(email ?? "eauclaire1510@gmail.com") },
                            Subject = "Your OTP Code to Verify Eau Claire account!",
                            Body = _templateService.GetEmailOtpTemplate(otp),
                            IsBodyHtml = true,

                        };

                        await client.SendMailAsync(mailMessage);

                        return new ServiceResult
                        {
                            IsSuccess = true,
                            ErrorCode = "200",
                            Message = "OTP sent successfully via Email",
                            Data = null

                        };
                    }

                }
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = "400",
                    Message = "Invalid method. Use 'sms' or 'email'.",
                    Data = null
                };

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

        public string VerifyOtp(string method, string inputOtp, int? userId, string deviceId, string? phone, string? email, string purpose)
        {
            var cacheKey = method == "sms" ? $"otp_{deviceId}_{phone}" : $"otp_{deviceId}_{email}";

            var storedOtp = _cache.Get<string>(cacheKey);

            if (storedOtp != null && storedOtp == inputOtp)
            {
                _cache.Remove(cacheKey);
                var tempToken = Guid.NewGuid().ToString();

                _cache.Set(tempToken, new TempTokenData
                {
                    DeviceId = deviceId,
                    UserId = userId ?? 0,
                    Phone = phone ?? "",
                    Email = email ?? "",
                    Method = method,
                    isVerified = false,
                    Purpose = purpose

                }, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

                return tempToken;
            }
            return null;
        }
    }
}
