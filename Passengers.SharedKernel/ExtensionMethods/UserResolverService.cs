using Passengers.SharedKernel.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Web.Common.Util
{

    public interface IUserResolverService
    {
        Tkey GetCurrentUserId<Tkey>();
    }

    public class UserResolverService : IUserResolverService
    {

        private readonly IHttpContextAccessor HttpContextAccessor;

        public UserResolverService(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        public Tkey GetCurrentUserId<Tkey>()
        {
            if (IsAuthenticated())
              return HttpContextAccessor.HttpContext.User.CurrentUserId<Tkey>();
            return default(Tkey);
        }


        private bool IsAuthenticated()
        {
            if (HttpContextAccessor?.HttpContext?.User != null)
                return HttpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            return false;
        }

    }
}
