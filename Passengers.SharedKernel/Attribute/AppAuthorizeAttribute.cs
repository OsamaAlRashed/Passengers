using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Attribute
{
    public class AppAuthorizeAttribute : AuthorizeAttribute
    {
        public AppAuthorizeAttribute()
        {
        }

        public AppAuthorizeAttribute(params string[] roles)
        {
            base.Roles = String.Join(",", roles);
        }
    }
}
