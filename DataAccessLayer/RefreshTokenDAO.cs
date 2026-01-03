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
        private readonly FishFarmContext _dbcontext;

        public RefreshTokenDAO (FishFarmContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public bool SaveRefreshToken(int userId, string deviceId, string refreshToken, DateTime expiryDate)
        {
            try
            {
                var tokenEntry = _dbcontext.RefreshTokens.FirstOrDefault(t => t.UserId == userId);
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
                    _dbcontext.RefreshTokens.Add(tokenEntry);
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
                var tokenEntry = _dbcontext.RefreshTokens.FirstOrDefault(t => t.UserId == userId && t.Token == refreshToken);
                //if (tokenEntry != null && !tokenEntry.IsExpired)
                //{
                //    return true;
                //}
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
