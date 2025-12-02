using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.Services
{
    public interface IUtils
    {
        public string GetEmailOtpTemplate(string otp);
    }
}
