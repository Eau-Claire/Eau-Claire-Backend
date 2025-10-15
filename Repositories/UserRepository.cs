using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.DataAccessLayer;

namespace FishFarm.Repositories
{
    public class UserRepository : IUserRepository
    {
        private UserDAO _userDAO = new UserDAO();
        public bool ForgetPassword(int id, string newPasswordHash)
        {
            return _userDAO.ForgetPassword(id, newPasswordHash);
        }

        public string GetUserRole(int id)
        {
            return _userDAO.GetUserRole(id);
        }

        public User? Login(string username, string passwordHash)
        {
            return _userDAO.Login(username, passwordHash);
        }

        public bool Register(string username, string passwordHash)
        {
            return _userDAO.Register(username, passwordHash);
        }
    }
}
