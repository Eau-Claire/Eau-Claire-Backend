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
        private readonly FishFarmContext _dbcontext ;

        public UserProfileDAO(FishFarmContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public UserProfile GetUserProfile(int userId)
        {
            return _dbcontext.UserProfiles.FirstOrDefault(u => u.UserId == userId);
        }
        public bool UpdateUserProfile(int userId, string fullName, DateTime dob)
        {
            var user = _dbcontext.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                bool created = _dbcontext.UserProfiles.Add(new UserProfile
                {
                    UserId = userId,
                    FullName = fullName,
                    Dob = dob
                }) != null;
            }
            else
            {

                user.FullName = fullName;
                user.Dob = dob;
            }
            _dbcontext.SaveChanges();
            return true;
        }
    }
}
