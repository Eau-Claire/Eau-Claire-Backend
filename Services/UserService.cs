using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace FishFarm.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserProfileRepository _userProfileRepository;
        private IConfiguration _configure;

        public UserService(IConfiguration configure)
        {
            _userRepository = new UserRepository();
            _userProfileRepository = new UserProfileRepository();
            _configure = configure;
        }
        public bool ForgetPassword(int id, string newPasswordHash)
        {
            return _userRepository.ForgetPassword(id, newPasswordHash);
        }

        public string GetUserRole(int id)
        {
            return _userRepository.GetUserRole(id);
        }

        public LoginResponse? Login(string username, string passwordHash)
        {
            var user = _userRepository.Login(username, passwordHash);
            var userProfile = _userProfileRepository.GetUserProfile(user.UserId);
            if (user == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configure["Jwt:Key"]);

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new[]
                        {
                            new System.Security.Claims.Claim("username", username),
                            new System.Security.Claims.Claim("role", GetUserRole(id: user.UserId)),
                            new System.Security.Claims.Claim("userId", user.UserId.ToString()),
                            new System.Security.Claims.Claim("fullName", userProfile?.FullName ?? ""),
                            new System.Security.Claims.Claim("contactAddress", userProfile?.ContactAddress ?? ""),
                            new System.Security.Claims.Claim("permanentAddress", userProfile?.PermanentAddress ?? ""),
                            new System.Security.Claims.Claim("phoneNumber", userProfile?.CurrentPhoneNumber ?? ""),
                            new System.Security.Claims.Claim("dateOfBirth", userProfile?.DateOfBirth.ToString() ?? ""),
                            new System.Security.Claims.Claim("email", user ?.Email ?? "")
                        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new LoginResponse
            {
                accessToken = accessToken,
                expiresIn = 3600,
                refreshExpiresIn = 86400,
                refreshToken = Guid.NewGuid().ToString(),
                tokenType = "Bearer",
                scope = "profile email",
                userId = user.UserId,
                userProfile = new UserProfile
                {
                    FullName = userProfile?.FullName ?? "",
                    ContactAddress = userProfile?.ContactAddress ?? "",
                    PermanentAddress = userProfile?.PermanentAddress ?? "",
                    CurrentPhoneNumber = userProfile?.CurrentPhoneNumber ?? "",
                    DateOfBirth = userProfile?.DateOfBirth ?? null                  
                }
            };
        }

        public bool Register(string username, string passwordHash)
        {
            return _userRepository.Register(username, passwordHash);
        }
    }
}
