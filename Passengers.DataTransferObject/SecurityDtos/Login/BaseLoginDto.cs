using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos.Login
{
    public class BaseLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public bool RemmberMe { get; set; }
        public UserTypes UserType { get; set; }
    }
}
