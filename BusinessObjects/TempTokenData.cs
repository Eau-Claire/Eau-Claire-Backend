using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class TempTokenData
    {
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Method { get; set; }

        public bool isVerified { get; set; }
    }
}
