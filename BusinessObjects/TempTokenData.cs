using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class TempTokenData
    {
        public int UserId { get; set; }
        [Required]
        public string DeviceId { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Method { get; set; } = null!;

        public bool isVerified { get; set; }
        [Required]
        public string Purpose { get; set; } = null!;
    }
}
