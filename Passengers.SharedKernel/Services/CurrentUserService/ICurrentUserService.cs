using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.SharedKernel.Services.CurrentUserService
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        //UserTypes? Type { get; }
    }
}
