using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;

namespace FishFarm.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserProfileRepository _userProfileRepository;

        public UserProfileService(UserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public UserProfile GetUserProfile(int userId)
        {
            return _userProfileRepository.GetUserProfile(userId);
        }

        public bool UpdateUserProfile(int userId, string fullName, string currentAddress, string permanentAddress, string curentPhoneNumber, DateOnly dob)
        {
            return _userProfileRepository.UpdateUserProfile(userId, fullName, currentAddress, permanentAddress, curentPhoneNumber, dob);
        }
    }
}
