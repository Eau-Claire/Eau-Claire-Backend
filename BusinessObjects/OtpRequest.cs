using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class OtpRequest
    {
        public string Method { get; set; } = "Not Set";
        public int UserId { get; set; }
        [Required]
        public string DeviceId { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? InputOtp { get; set; }

        public string? Purpose { get; set; }

    }
}
