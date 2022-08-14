using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.Attribute
{
    public class AppAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private PassengersDbContext dbContext;

        public AppAuthorizeAttribute(params string[] roles)
        {
            base.Roles = String.Join(",", roles);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            dbContext = (PassengersDbContext)context.HttpContext.RequestServices.GetService(typeof(PassengersDbContext));

            var userId = context?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypes.NameIdentifier)?.FirstOrDefault()?.Value;
            if(userId != null)
            {
                var user = await dbContext.Users.IgnoreQueryFilters().Where(x => x.Id.ToString() == userId).FirstOrDefaultAsync();
                if(user is not null && !user.DateDeleted.HasValue && !user.DateBlocked.HasValue)
                    return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
