using Passengers.SharedKernel.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Passengers.SharedKernel.Enums;

namespace Passengers.SharedKernel.Services.CurrentUserService
{
    public class CurrentUserService  : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value?.StringToGuid();

        //public UserTypes? Type => _httpContextAccessor.HttpContext?.User?.FindFirst("Type")?.Value?.
    }
}
