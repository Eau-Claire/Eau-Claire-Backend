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
        private UserDAO _userDAO;

        public UserRepository(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }
        public bool ResetPassword(int id, string newPasswordHash)
        {
            return _userDAO.ResetPassword(id, newPasswordHash);
        }

        public User GetUserInfo(int id)
        {
            return _userDAO.GetUserInfo(id);
        }

        public User GetUserByUsername(string username)
        {
            return _userDAO.GetUserByUsername(username);
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
