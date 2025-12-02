using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.Services
{
    public class Utils : IUtils
    {
        public string GetEmailOtpTemplate(string otp)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "OtpTemplate.html");
            string htmlContent = File.ReadAllText(templatePath);

            var otpSpans = string.Join("", otp.Select(c => $"<span>{c}</span>"));

            htmlContent = htmlContent.Replace("{OTP}", otpSpans);
            return htmlContent;
        }
    }
}
