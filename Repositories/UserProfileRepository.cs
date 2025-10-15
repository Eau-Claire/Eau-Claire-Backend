using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.DataAccessLayer;

namespace FishFarm.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private UserProfileDAO _userProfileDAO = new UserProfileDAO();

        public UserProfile GetUserProfile(int userId)
        {
            return _userProfileDAO.GetUserProfile(userId);
        }

        public bool UpdateUserProfile(int userId, string fullName, string currentAddress, string permanentAddress, string curentPhoneNumber, DateOnly dob)
        {
            return _userProfileDAO.UpdateUserProfile(userId, fullName, currentAddress, permanentAddress, curentPhoneNumber, dob);
        }
    }
}
