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

        public User GetUserInfo(int id)
        {
            return _userRepository.GetUserInfo(id);
        }

        public LoginResponse? Login(string username, string password, string deviceId)
        {
            try
            {
                var user = _userRepository.Login(username, password);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid username or password",
                        isDeviceVerified = false,
                    };
                }

                var userProfile = _userProfileRepository.GetUserProfile(user.UserId);
                var userInfo = _userRepository.GetUserInfo(user.UserId);


                var isVerified = _deviceService.CheckDeviceIsVerified(deviceId, user.UserId);

                if (!isVerified)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Device is not verified",
                        isDeviceVerified = false,
                    };
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configure["Jwt:Key"] ?? "");

                var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(
                        new[]
                            {
                            new System.Security.Claims.Claim("username", username),
                            new System.Security.Claims.Claim("role", userInfo.Role ?? ""),
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
                    status = "200",
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

            }catch (Exception ex)
            {
                return new LoginResponse 
                {
                    status = "500",
                    message = "An error occurred during login",
                    isDeviceVerified = false,

                };
            }
        }

        public UserInfoResponse GetUserInfoByUsername (string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            return new UserInfoResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Phone = user.Phone ?? "",
                Email = user.Email ?? "",
                Role = user.Role ?? ""
            };
        }

        public LoginResponse ResetPassword(int userId, string newPassword, string confirmPassword, string tempToken)
        {
            try
            {
                if(string.Compare(newPassword, confirmPassword) != 0)
                {
                    return new LoginResponse
                    {
                        status = "400",
                        message = "New password and confirm password do not match",
                        isDeviceVerified = false,
                    };
                }

                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken) ?? new TempTokenData();
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken == null)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid token",
                        isDeviceVerified = false,
                    };
                }

                var isSuccess = _userRepository.ResetPassword(userId, newPassword);

                if (isSuccess)
                {
                    return new LoginResponse
                    {
                        status = "200",
                        message = "Password updated successfully",
                        isDeviceVerified = true,
                    };
                } 

                return new LoginResponse
                {
                    status = "500",
                    message = "Failed to update password",
                    isDeviceVerified = false,
                };

            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = ex.Message,
                    isDeviceVerified = false
                };
            }
        }

        public LoginResponse ValidateTempToken (string tempToken)
        {
            try
            {
                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken) ?? new TempTokenData();
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken == null)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid token",
                        isDeviceVerified = false,
                    };
                }

                var userProfile = _userProfileRepository.GetUserProfile(userToken.UserId);
                var userInfo = _userRepository.GetUserInfo(userToken.UserId);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configure["Jwt:Key"]);

                var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(
                        new[]
                            {
                            new System.Security.Claims.Claim("username", userInfo.Username),
                            new System.Security.Claims.Claim("role", userInfo.Role ?? ""),
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

                var device = _deviceService.AddOrUpdateDeviceIsVerified(userToken.DeviceId, userToken.UserId, "", "");

                if (device == null)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Device is not verified",
                        isDeviceVerified = false,
                    };
                }

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token);

                return new LoginResponse
                {
                    status = "200",
                    accessToken = accessToken,
                    expiresIn = 3600,
                    refreshExpiresIn = 86400,
                    refreshToken = Guid.NewGuid().ToString(),
                    tokenType = "Bearer",
                    scope = "profile email",
                    userId = userToken.UserId,
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
                    status = "500",
                    message = "Invalid token",
                    isDeviceVerified = false,
                };
            }
        }
    }
}
