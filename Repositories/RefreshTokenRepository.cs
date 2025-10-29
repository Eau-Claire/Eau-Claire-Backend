using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.DataAccessLayer;

namespace FishFarm.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RefreshTokenDAO _refreshTokenDAO ;

        public RefreshTokenRepository (RefreshTokenDAO refreshTokenDAO)
        {
            _refreshTokenDAO = refreshTokenDAO;
        }

        public bool isValidRefreshToken(int userId, string refreshToken)
        {
            return _refreshTokenDAO.isValidRefreshToken(userId, refreshToken);
        }

        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiryDate)
        {
            return _refreshTokenDAO.SaveRefreshToken(userId, refreshToken, expiryDate);
        }
    }
}
