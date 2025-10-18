using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.BusinessObjects
{
    public class LoginResponse
    {
        public string status { get; set; }
        public string accessToken { get; set; }
        public int expiresIn { get; set; } = 3600;
        public int refreshExpiresIn { get; set; } = 86400;
        public string refreshToken { get; set; }
        public string tokenType { get; set; } = "Bearer";

        public string scope { get; set; } = "profile email";
        public int userId { get; set; }
        public bool isDeviceVerified { get; set; }
        public UserProfile userProfile { get; set; } = null!;
    }
}
