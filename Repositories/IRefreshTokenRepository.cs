using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishFarm.Repositories
{
    public interface IRefreshTokenRepository
    {
        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiryDate);

        public bool isValidRefreshToken(int userId, string refreshToken);
    }
}
