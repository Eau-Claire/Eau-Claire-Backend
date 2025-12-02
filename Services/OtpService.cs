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
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio.Rest.Api.V2010.Account;

namespace FishFarm.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        private readonly string _sendGridAppPassword;
        private readonly IUtils _utils;

        public OtpService(IMemoryCache cache, IUtils utils)
        {
            _accountSid = Environment.GetEnvironmentVariable("Twilio__AccountSid") ?? "";
            _authToken = Environment.GetEnvironmentVariable("Twilio__AuthToken") ?? "";
            _fromPhoneNumber = Environment.GetEnvironmentVariable("Twilio__FromPhoneNumber") ?? "";

            _sendGridAppPassword = Environment.GetEnvironmentVariable("SendGrid__SendGridPassword") ?? "";

            _cache = cache;
            _utils = utils;
        }
        private static string GenerateOtp(int length = 6)
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
        public async Task<ServiceResult> SendOtp(string method, string deviceId, string? phone, string? email)
        {
            try
            {
                string cacheKey = method == "sms" ? $"otp_{deviceId}_{phone}" : $"otp_{deviceId}_{email}";
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                if (_cache.TryGetValue(cacheKey, out _))
                {
                    _cache.Remove(cacheKey);
                }

                string otp = GenerateOtp(6);

                if (method == "sms")
                {
                    Console.WriteLine("hay: " + _accountSid + " " + _authToken + "" + _fromPhoneNumber);

                    _cache.Set(cacheKey, otp, cacheOptions);

                    Twilio.TwilioClient.Init(_accountSid, _authToken);
                    await MessageResource.CreateAsync(
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
                else
                {
                    _cache.Set(cacheKey, otp, cacheOptions);

                    var client = new SendGridClient(_sendGridAppPassword);
                    var from = new EmailAddress("eauclaire1510@gmail.com", "Eau Claire Support");
                    var subject = "Your OTP Code to Verify Eau Claire account!";
                    var to = new EmailAddress(email ?? "eauclaire1510@gmail.com");
                    var htmlContent = _utils.GetEmailOtpTemplate(otp);
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, $"Your OTP code is {otp}", htmlContent);
                    await client.SendEmailAsync(msg);

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

        public ServiceResult VerifyOtp(string method, string inputOtp, int? userId, string deviceId, string? phone, string? email, string purpose)
        {
            string cacheKey = method == "sms" ? $"otp_{deviceId}_{phone}" : $"otp_{deviceId}_{email}";

            string storedOtp = _cache.Get<string>(cacheKey);

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

                return new ServiceResult
                {
                    IsSuccess = true,
                    ErrorCode = "200",
                    Message = $"Success verifying OTP.",
                    Data = tempToken

                };
            }
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorCode = "500",
                Message = $"Failed to verify OTP.",
                Data = null

            };
        }
    }
}
