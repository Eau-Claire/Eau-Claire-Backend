using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class CreateUserResponse
    {
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
