using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }

        public bool IsActive => Revoked == null && !IsExpired;

        public virtual User User { get; set; } = null!;

    }
}
