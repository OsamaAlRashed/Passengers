using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.Shared.Store
{
    public class SecurityStore
    {
        public static class Filter
        {
        }
        public static class Query
        {
            public static Func<LoginMobileDto, BaseLoginDto> ShopToBaseLoginDto => c => new BaseLoginDto
            {
                UserName = c.UserName,
                DeviceToken = c.DeviceToken,
                Password = c.Password,
                RemmberMe = false,
                UserType = UserTypes.Shop
            };

        }
    }
}
