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

        public string GetUserRole(int id)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == id);

            if (user != null)
            {
                return user.Role ?? "N";
            }
            return "N/A";
        }

        public bool Login(string username, string passwordHash)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user != null;
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

        public bool ForgetPassword(int id, string newPasswordHash)
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
