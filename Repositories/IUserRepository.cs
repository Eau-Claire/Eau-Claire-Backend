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
        public string GetUserRole(int id);
        public User? Login(string username, string passwordHash);
        public bool Register(string username, string passwordHash);
        public bool ForgetPassword(int id, string newPasswordHash);
    }
}
