using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FishFarm.Services
{
    public class OtpService
    {
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

        public static bool SendOtp(string phoneNumber, string otp)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("eauclaire1510@gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("eauclaire1510@gmail.com", "12345@abc");
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("eauclaire1510@gmail.com"),
                        To = { new MailAddress("eauclaire1510@gmail.com") },
                        Subject = "Your OTP Code to Verify Eau Claire account!",
                        Body = $"Your OTP code is: {otp}",
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

        public static bool VerifyOtp(string inputOtp, string actualOtp)
        {
            return inputOtp == actualOtp;
        }
    }
}
