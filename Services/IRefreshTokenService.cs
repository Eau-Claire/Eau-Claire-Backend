using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.Services
{
    public interface IRefreshTokenService
    {
        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiryDate);
        public bool isValidRefreshToken(int userId, string refreshToken);
    }
}
