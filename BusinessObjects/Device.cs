using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class Device
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
        public virtual User User { get; set; } = null!;
    }
}
