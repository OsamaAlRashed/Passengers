using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Passengers.Models.Security;
using Passengers.Repository.Base;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.TokenService
{
    public class TokenService : BaseRepository, ITokenService
    {
        private readonly IConfiguration configuration;
        public TokenService(PassengersDbContext context, IConfiguration configuration): base(context)
        {
            this.configuration = configuration;
        }

        public string GenerateAccessToken(AppUser user, IList<string> roles, DateTime expierDate)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(AppCliams.Type, user.UserType.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                  configuration["Jwt:Issuer"],
                  claims,
                  expires: expierDate,
                  signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> AddRefreshToken(Guid userId)
        {
            try
            {
                var refreshToken = new RefreshToken
                {
                    Token = GenerateRefreshToken(),
                    UserId = userId
                };
                Context.RefreshTokens.Add(refreshToken);
                await Context.SaveChangesAsync();

                return refreshToken.Token;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public async Task<string> UpdateRefreshToken(Guid userId, string oldRefreshToken)
        {
            await DeleteRefreshToken(oldRefreshToken);
            return await AddRefreshToken(userId);
        }

        public async Task<bool> DeleteRefreshToken(string oldRefreshToken)
        {
            try
            {
                var oldRefreshTokenEntity = await Context.RefreshTokens.Where(x => x.Token == oldRefreshToken).FirstOrDefaultAsync();
                if (oldRefreshTokenEntity != null)
                {
                    Context.RefreshTokens.Remove(oldRefreshTokenEntity);

                    await Context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task<List<string>> Get(Guid? userId, bool isExpired = false)
        {
            var tokens = (await Context.RefreshTokens.ToListAsync())
                .Where(x => (x.IsExpired == isExpired) && (!userId.HasValue || x.UserId == userId))
                .Select(x => x.Token)
                .ToList();
            return tokens;
        }

        public async Task<bool> DeleteExpired(Guid? userId)
        {
            try
            {
                var tokens = await Get(userId, true);
                Context.RemoveRange(tokens);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
