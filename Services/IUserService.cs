using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.Services
{
    public interface IUserService
    {
        public User GetUserInfo(int id);
        public User GetUserInfoByUsername(string username);
        public LoginResponse? Login(string username, string password, string deviceId);
        public LoginResponse ResetPassword(int userId, string newPassword, string confirmPassword, string tempToken);
        public LoginResponse ValidateTempToken(string tempToken);
        public LoginResponse ValidateRegistrationTempToken(string tempToken);
        public bool ValidateGenericTempToken(string tempToken);

        public LoginResponse GetNewAccessTokenIfRefreshTokenValid(int userId, string refreshToken, string method);
    }
}
