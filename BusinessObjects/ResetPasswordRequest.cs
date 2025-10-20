using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class ResetPasswordRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; } 
        public string ConfirmPassword { get; set; } 
        public string TempToken { get; set; } 
    }
}
