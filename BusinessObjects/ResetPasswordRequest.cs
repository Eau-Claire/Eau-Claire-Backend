using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class ResetPasswordRequest
    {
        public int UserId { get; set; }
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
        [Required]
        public string TempToken { get; set; } = null!;
    }
}
