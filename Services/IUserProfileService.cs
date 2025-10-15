using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Services
{
    public interface IUserProfileService
    {
        public UserProfile GetUserProfile(int userId);
        public bool UpdateUserProfile(int userId, string fullName, string currentAddress, string permanentAddress, string curentPhoneNumber, DateOnly dob);
    }
}
