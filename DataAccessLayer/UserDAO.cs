using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.DataAccessLayer
{
    public class UserDAO
    {
        private FishFarmDbV2Context _dbcontext = new FishFarmDbV2Context();

        public User GetUserInfo(int id)
        {
            return _dbcontext.Users.FirstOrDefault(u => u.UserId == id);
        }

        public User GetUserByUsername(string username)
        {
            return _dbcontext.Users.FirstOrDefault(u => u.Username == username);
        }

        public User? Login(string username, string passwordHash)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user;
        }

        public bool Register(string username, string passwordHash)
        {
            var existingUser = _dbcontext.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                return false;
            }
            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Role = "Guest"
            };

            _dbcontext.Users.Add(newUser);
            _dbcontext.SaveChanges();
            return true;
        }

        public bool ResetPassword(int id, string newPasswordHash)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return false;
            }
            user.PasswordHash = newPasswordHash;
            _dbcontext.SaveChanges();
            return true;
        }
    }
}
