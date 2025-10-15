using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.DataAccessLayer
{
    public class UserProfileDAO
    {
        private FishFarmDbV2Context _dbcontext = new FishFarmDbV2Context();

        public UserProfile GetUserProfile(int userId)
        {
            return _dbcontext.UserProfiles.FirstOrDefault(u => u.UserId == userId);
        }
        public bool UpdateUserProfile(int userId, string fullName, string currentAddress, string permanentAddress, string curentPhoneNumber, DateOnly dob)
        {
            var user = _dbcontext.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }
            user.FullName = fullName;
            user.ContactAddress = currentAddress;
            user.ContactAddress = permanentAddress;
            user.CurrentPhoneNumber = curentPhoneNumber;
            user.DateOfBirth = dob;
            _dbcontext.SaveChanges();
            return true;
        }
    }
}
