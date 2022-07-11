using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Constants.Security
{
    public static class ConstantValue
    {
        public const int ExpireAccessTokenMinute = 2;
        public static DateTime AccessExpireDateTime = DateTime.Now.AddDays(ExpireAccessTokenMinute);

        public const int ExpireRefreshTokenDay = 60;
        public static DateTime RefreshExpireDateTime = DateTime.Now.AddDays(ExpireRefreshTokenDay);
    }

    public class AppRoles
    {
        public const string Admin = "Admin";
        public const string Shop = "Shop";
        public const string Customer = "Customer";
        public const string Driver = "Driver";
    }

    public static class AppCliams
    {
        public const string Type = "Type";
    }
}
