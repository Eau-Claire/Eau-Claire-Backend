using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace FishFarm.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        private readonly TemplateService _templateService;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
            _templateService = new TemplateService();
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

        public string FindExistedOtp(string method, string? phone, string? email)
        {
            var cacheKey = method == "sms" ? $"otp_{phone}" : $"otp_{email}";
            return _cache.Get<string>(cacheKey) ?? "";
        }

        public bool SendOtp(string method, string otp, string? phone, string? email)
        {
            try
            {
                var cacheKey = string.Empty;
                var cacheOptions = new MemoryCacheEntryOptions();

                string existedOtp = FindExistedOtp(method, phone, email);

                if (existedOtp != null && existedOtp != "")
                {
                    _cache.Remove(cacheKey);
                }

                if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(otp))
                {
                    throw new ArgumentException("Method and OTP must be provided");
                }

                if (method == "email" && string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email must be provided for email method");
                }

                if (method == "sms" && string.IsNullOrEmpty(phone))
                {
                    throw new ArgumentException("Phone number must be provided for SMS method");
                }

                if (method == "sms")
                {
                    cacheKey = $"otp_{phone}";
                    cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };

                    _cache.Set(cacheKey, otp, cacheOptions);

                    return true;
                }

                cacheKey = $"otp_{email}";
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

                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP: {ex.Message}");
                return false;

            }
        }

        public bool VerifyOtp(string method, string inputOtp, string? phone, string? email)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(inputOtp))
            {
                throw new ArgumentException("Method and input OTP must be provided");
            }

            var cacheKey = method == "sms" ? $"otp_{phone}" : $"otp_{email}";

            if (_cache.TryGetValue(cacheKey, out string? actualOtp))
            {
                if (inputOtp == actualOtp)
                {
                    _cache.Remove(cacheKey);
                    return true;
                }
               
            }
            return false;
        }
    }
}
