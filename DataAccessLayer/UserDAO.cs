using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FishFarm.DataAccessLayer
{
    public class UserDAO
    {
        private readonly FishFarmContext _dbcontext;

        public UserDAO(FishFarmContext dbcontext)
        {
            _dbcontext = dbcontext;
            Console.WriteLine(_dbcontext.Database.GetConnectionString());
        }

        public User GetUserInfo(int id)
        {
            return _dbcontext.Users.FirstOrDefault(u => u.UserId == id) ;
        }

        public User GetUserByUsername(string username)
        {
            return _dbcontext.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool IsUserExisted(string username)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Username == username);
            return user != null;
        }

        public User? Login(string username, string passwordHash)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user;
        }

        public bool Register(string username, string passwordHash, int storeId)
        {
            var existingUser = _dbcontext.Users.FirstOrDefault(u => u.Username == username);
            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            var userStoreRole = new UserStoreRole
            {
                UserId = newUser.UserId,
                StoreId = storeId
            };

            _dbcontext.Users.Add(newUser);
            _dbcontext.UserStoreRoles.Add(userStoreRole);
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
