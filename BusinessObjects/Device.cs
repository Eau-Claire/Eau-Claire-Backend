using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public partial class Device
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceName { get; set; } = "Not Set";
        public string DeviceType { get; set; } = "Not Set";
        [Required]
        public string DeviceId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = true;
        
        public virtual User User { get; set; } = null!;
    }
}
