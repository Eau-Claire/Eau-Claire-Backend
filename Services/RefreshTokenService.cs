using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.Repositories;

namespace FishFarm.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(RefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiryDate)
        {
            return _refreshTokenRepository.SaveRefreshToken(userId, refreshToken, expiryDate);
        }

        public bool isValidRefreshToken(int userId, string refreshToken)
        {
            return _refreshTokenRepository.isValidRefreshToken(userId, refreshToken);
        }

    }
}
