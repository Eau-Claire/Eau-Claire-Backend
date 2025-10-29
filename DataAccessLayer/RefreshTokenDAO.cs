using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishFarm.BusinessObjects;

namespace FishFarm.DataAccessLayer
{
    public class RefreshTokenDAO
    {
        private FishFarmDbV2Context _dbcontext;

        public RefreshTokenDAO (FishFarmDbV2Context dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public bool SaveRefreshToken(int userId, string refreshToken, DateTime expiryDate)
        {
            try
            {
                var tokenEntry = _dbcontext.RefreshToken.FirstOrDefault(t => t.UserId == userId);
                if (tokenEntry != null)
                {
                    tokenEntry.Token = refreshToken;
                    tokenEntry.Expires = expiryDate;
                    tokenEntry.Created = DateTime.UtcNow;

                }
                else
                {
                    tokenEntry = new RefreshToken
                    {
                        UserId = userId,
                        Token = refreshToken,
                        Expires = expiryDate,
                        Created = DateTime.UtcNow
                    };
                    _dbcontext.RefreshToken.Add(tokenEntry);
                }
                _dbcontext.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;


            }
        }

        public bool isValidRefreshToken(int userId, string refreshToken)
        {
            try
            {
                var tokenEntry = _dbcontext.RefreshToken.FirstOrDefault(t => t.UserId == userId && t.Token == refreshToken);
                if (tokenEntry != null && !tokenEntry.IsExpired)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
