using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FishFarm.BusinessObjects;
using FishFarm.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using BCrypt.Net;

namespace FishFarm.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileService _userProfileService;
        private readonly IDeviceService _deviceService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMemoryCache _cache;

        public UserService(IMemoryCache cache,
            IUserRepository userRepository, IUserProfileService userProfileService,
            IDeviceService deviceService, IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _userProfileService = userProfileService;
            _deviceService = deviceService;
            _refreshTokenService = refreshTokenService;
            _cache = cache;
        }

        public User GetUserInfo(int id)
        {
            return _userRepository.GetUserInfo(id);
        }
        public User GetUserInfoByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public LoginResponse GetNewAccessTokenIfRefreshTokenValid(int userId, string refreshToken, string method)
        {
            try
            {
                var isValid = _refreshTokenService.isValidRefreshToken(userId, refreshToken);
                if (!isValid)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid refresh token",
                        isDeviceVerified = false,
                    };
                }
                var user = _userRepository.GetUserInfo(userId);
                var userProfile = _userProfileService.GetUserProfile(userId);

                return GenerateTokenResponse(user, userProfile, method);
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = $"An error occurred while generating new access token: {ex.Message}",
                    isDeviceVerified = false,
                };
            }
        }

        private LoginResponse GenerateTokenResponse(User user, UserProfile userProfile, string method)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBase64 = Environment.GetEnvironmentVariable("Jwt__Key");

            if (string.IsNullOrEmpty(keyBase64))
            {
                return new LoginResponse
                {
                    status = "500",
                    message = "JWT Key is not configured properly",
                    isDeviceVerified = false,
                };
            }

            byte[] keyBytes;

            try
            {
                keyBytes = Convert.FromBase64String(keyBase64);

            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = $"Error while processing JWT Key: {ex.Message}"
                };
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new[]
                        {
                            new System.Security.Claims.Claim("username", user.Username ?? ""),
                            new System.Security.Claims.Claim("role", user.Role.ToString()),
                            new System.Security.Claims.Claim("userId", user.UserId.ToString()),
                            new System.Security.Claims.Claim("fullName", userProfile?.FullName ?? ""),
                            new System.Security.Claims.Claim("contactAddress", userProfile?.ContactAddress ?? ""),
                            new System.Security.Claims.Claim("permanentAddress", userProfile?.PermanentAddress ?? ""),
                            new System.Security.Claims.Claim("phoneNumber", userProfile?.CurrentPhoneNumber ?? ""),
                            new System.Security.Claims.Claim("dateOfBirth", userProfile?.DateOfBirth.ToString() ?? ""),
                            new System.Security.Claims.Claim("email", user.Email ?? "")
                        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = Environment.GetEnvironmentVariable("Jwt__Issuer"),
                Audience = Environment.GetEnvironmentVariable("Jwt__Audience")
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();

            if (user.UserId == 0)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = "Invalid user information",
                    isDeviceVerified = false,
                };
            }

            if (!_refreshTokenService.SaveRefreshToken(user.UserId, refreshToken, DateTime.UtcNow.AddDays(1)))
            {
                return new LoginResponse
                {
                    status = "500",
                    message = "Failed to generate refresh token",
                    isDeviceVerified = false,
                };
            }

            return new LoginResponse
            {
                status = "200",
                accessToken = accessToken,
                expiresIn = 3600,
                refreshExpiresIn = 86400,
                refreshToken = refreshToken,
                tokenType = "Bearer",
                scope = $"profile {method}", //sua sau
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

        // sau khi tao them API cho customer thi chuyen cai nay qua customer service
        public LoginResponse? Register(string username, string password, string? phone, string? email, string deviceId, string tempToken)
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
                else if (userToken.isVerified == false)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Token not verified for registration",
                        isDeviceVerified = false,
                    };
                }
                else if (userToken.Purpose != "register")
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid token purpose for registration",
                        isDeviceVerified = false,
                    };
                }

                bool isUserExists = _userRepository.IsUserExisted(username);

                if (isUserExists == true)
                {
                    return new LoginResponse
                    {
                        status = "409",
                        message = "User with the same email or phone already exists",
                        isDeviceVerified = false,
                    };
                }

                _cache.Remove(tempToken);

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                bool isRegisterSuccess = _userRepository.Register(username, passwordHash);
                
                if (!isRegisterSuccess)
                {
                    return new LoginResponse
                    {
                        status = "500",
                        message = "Registration failed due to internal error",
                        isDeviceVerified = false,
                    };
                }
                
                var user = _userRepository.GetUserByUsername(username);


                _cache.Remove(tempToken);

                //Goi genrate token response de tra ve cho client neu dang ky thanh cong
                return GenerateTokenResponse(user, new UserProfile(), "registration");
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = "An error occurred during registration",
                    isDeviceVerified = false,
                };
            }
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

                var userProfile = _userProfileService.GetUserProfile(user.UserId);

                var isVerified = _deviceService.CheckDeviceIsVerified(deviceId, user.UserId);

                if (!isVerified)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Device is not verified",
                        userId = user.UserId,
                        isDeviceVerified = false,
                    };
                }

                return GenerateTokenResponse(user, userProfile, "direct login");

            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = $"An error occurred during login: {ex.Message} ",
                    isDeviceVerified = false,

                };
            }
        }
        public LoginResponse ResetPassword(int userId, string newPassword, string confirmPassword, string tempToken)
        {
            try
            {
                if (string.Compare(newPassword, confirmPassword) != 0)
                {
                    return new LoginResponse
                    {
                        status = "400",
                        message = "New password and confirm password do not match",
                        isDeviceVerified = false,
                    };
                }

                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken)!;
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                bool isVerified = userToken.isVerified;

                if (!isVerified)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Token not verified for password reset, please verify it first",
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

        //Validate temp token generated after OTP verification for Login
        public LoginResponse ValidateTempToken(string tempToken)
        {
            try
            {
                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken)!;
                if (userToken == null)
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid token",
                        isDeviceVerified = false,
                    };
                }

                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken.Purpose != "login" && userToken.Purpose != "register")
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid purpose",
                        isDeviceVerified = false,
                    };
                }

                var userProfile = _userProfileService.GetUserProfile(userToken.UserId);
                var user = _userRepository.GetUserInfo(userToken.UserId);

                if (user == null || user.UserId == 0)
                {
                    return new LoginResponse
                    {
                        status = "500",
                        message = "Invalid user information",
                        isDeviceVerified = false,
                    };
                }

                if (userToken.Purpose == "login")
                {
                    Device device = _deviceService.AddOrUpdateDeviceIsVerified(userToken.DeviceId, userToken.UserId, "", "");

                    if (device == null)
                    {
                        return new LoginResponse
                        {
                            status = "500",
                            message = "Failed to verify device",
                            isDeviceVerified = false,
                        };
                    }

                    _cache.Remove(tempToken);

                    return GenerateTokenResponse(user, userProfile, userToken.Method);

                }
                else if (userToken.Purpose == "register")
                {
                    //Missing item: register user function
                }

                return new LoginResponse
                {
                    status = "500",
                    message = "Invalid token purpose",
                    isDeviceVerified = false,
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = $"Error occures while verifying Temporal Token: {ex.Message}",
                    isDeviceVerified = false,
                };
            }
        }


        //Validate temp token generated after OTP verification for Register
        public LoginResponse ValidateRegistrationTempToken(string tempToken)
        {
            try
            {
                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken)!;
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken.Purpose != "register")
                {
                    return new LoginResponse
                    {
                        status = "401",
                        message = "Invalid token",
                        isDeviceVerified = false,
                    };
                }

                userToken.isVerified = true;

                return new LoginResponse
                {
                    status = "200",
                    message = "Token is valid for registration",
                    isDeviceVerified = true,
                };

            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    status = "500",
                    message = $"Error occures while verifying Temperal Token: {ex.Message}",
                    isDeviceVerified = false,
                };
            }
        }

        //Validate temp token for other functions if needed in future
        public bool ValidateGenericTempToken(string tempToken)
        {
            try
            {
                TempTokenData userToken = _cache.Get<TempTokenData>(tempToken)!;
                Console.WriteLine(JsonConvert.SerializeObject(userToken));

                if (userToken.Purpose != "generic")
                {
                    return false;
                }

                userToken.isVerified = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
