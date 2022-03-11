using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Constants.Security
{
    public class ConstantValue
    {
        public const int DefaultExpireTokenMinute = 1 * 24 * 60;
    }

    public class AppRoles
    {
        public const string Admin = "Admin";
        public const string Shop = "Shop";
        public const string Customer = "Customer";
        public const string Driver = "Driver";
        public const string Stakeholder = "Stakeholder";
    }
}
