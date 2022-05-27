using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.TokenService
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user, IList<string> roles, DateTime expierDate);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();

        Task<string> AddRefreshToken(Guid userId);
        Task<string> UpdateRefreshToken(Guid userId, string oldRefreshToken);
        Task<bool> DeleteRefreshToken(string oldRefreshToken);
        Task<List<string>> Get(Guid? userId, bool isExpired = false);
        Task<bool> DeleteExpired(Guid? userId);
    }
}
