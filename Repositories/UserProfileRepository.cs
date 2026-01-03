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
        private readonly UserProfileDAO _userProfileDAO;

        public UserProfileRepository(UserProfileDAO userProfileDAO)
        {
            _userProfileDAO = userProfileDAO;
        }

        public UserProfile GetUserProfile(int userId)
        {
            return _userProfileDAO.GetUserProfile(userId);
        }

        public bool UpdateUserProfile(int userId, string fullName, DateTime dob)
        {
            return _userProfileDAO.UpdateUserProfile(userId, fullName, dob);
        }
    }
}
