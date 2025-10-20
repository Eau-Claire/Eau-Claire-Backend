using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Repositories
{
    public interface IUserRepository
    {
        public User GetUserInfo(int id);
        public User GetUserByUsername(string username);
        public User? Login(string username, string passwordHash);
        public bool Register(string username, string passwordHash);
        public bool ResetPassword(int id, string newPasswordHash);
    }
}
