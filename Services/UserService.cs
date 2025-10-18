using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FishFarm.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserProfileRepository _userProfileRepository;
        private readonly DeviceService _deviceService;
        private readonly IMemoryCache _cache;
        private IConfiguration _configure;

        public UserService(IMemoryCache cache, IConfiguration configure)
        {
            _userRepository = new UserRepository();
            _userProfileRepository = new UserProfileRepository();
            _deviceService = new DeviceService();
            _configure = configure;
            _cache = cache;
        }
        public bool ForgetPassword(int id, string newPassword)
        {
            return _userRepository.ForgetPassword(id, newPassword);
        }

        public string GetUserRole(int id)
        {
            return _userRepository.GetUserRole(id);
        }

        public LoginResponse? Login(string username, string password, string deviceId)
        {
            var user = _userRepository.Login(username, password);
            var userProfile = _userProfileRepository.GetUserProfile(user.UserId);
            if (user == null)
            {
                return null;
            }

            var isVerified = _deviceService.CheckDeviceIsVerified(deviceId, user.UserId.ToString());

            if(!isVerified)
            {
                return new LoginResponse
                {
                    status = "Device not verified",
                    accessToken = "",
                    expiresIn = 0,
                    refreshExpiresIn = 0,
                    refreshToken = "",
                    tokenType = "Bearer",
                    scope = "",
                    userId = user.UserId,
                    isDeviceVerified = false,
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
                status = "success",
                accessToken = accessToken,
                expiresIn = 3600,
                refreshExpiresIn = 86400,
                refreshToken = Guid.NewGuid().ToString(),
                tokenType = "Bearer",
                scope = "profile email",
                userId = user.UserId,
                isDeviceVerified = true,
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

        public bool Register(string username, string password)
        {
            return _userRepository.Register(username, password);
        }

        public LoginResponse ValidateTempToken (string tempToken)
        {
            try
            {
                var userToken = _cache.Get<dynamic>(tempToken);
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken == null)
                {
                    return new LoginResponse
                    {
                        status = "Invalid token",
                        accessToken = "",
                        expiresIn = 0,
                        refreshExpiresIn = 0,
                        refreshToken = "",
                        tokenType = "Bearer",
                        scope = "",
                        userId = 0,
                        isDeviceVerified = false,
                        userProfile = new UserProfile()
                    };
                }

                var userProfile = _userProfileRepository.GetUserProfile(userToken.UserId);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configure["Jwt:Key"]);

                var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(
                        new[]
                            {
                            new System.Security.Claims.Claim("role", GetUserRole(id: userToken.UserId)),
                            new System.Security.Claims.Claim("userId", userToken.UserId.ToString()),
                            new System.Security.Claims.Claim("fullName", userProfile?.FullName ?? ""),
                            new System.Security.Claims.Claim("contactAddress", userProfile?.ContactAddress ?? ""),
                            new System.Security.Claims.Claim("permanentAddress", userProfile?.PermanentAddress ?? ""),
                            new System.Security.Claims.Claim("phoneNumber", userProfile?.CurrentPhoneNumber ?? ""),
                            new System.Security.Claims.Claim("dateOfBirth", userProfile?.DateOfBirth.ToString() ?? ""),
                            new System.Security.Claims.Claim("email", userToken ?.Email ?? "")
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
                    status = "success",
                    accessToken = accessToken,
                    expiresIn = 3600,
                    refreshExpiresIn = 86400,
                    refreshToken = Guid.NewGuid().ToString(),
                    tokenType = "Bearer",
                    scope = "profile email",
                    userId = userToken?.UserId,
                    isDeviceVerified = true,
                    userProfile = new UserProfile
                    {
                        FullName = userProfile?.FullName ?? "",
                        ContactAddress = userProfile?.ContactAddress ?? "",
                        PermanentAddress = userProfile?.PermanentAddress ?? "",
                        CurrentPhoneNumber = userProfile?.CurrentPhoneNumber ?? "",
                        DateOfBirth = userProfile?.DateOfBirth ?? null
                    }
                };

            } catch (Exception e)
            {
                return new LoginResponse
                {
                    status = "Invalid token",
                    accessToken = "",
                    expiresIn = 0,
                    refreshExpiresIn = 0,
                    refreshToken = "",
                    tokenType = "Bearer",
                    scope = "",
                    userId = 0,
                    isDeviceVerified = false,
                    userProfile = new UserProfile()
                };
            }
        }
    }
}
