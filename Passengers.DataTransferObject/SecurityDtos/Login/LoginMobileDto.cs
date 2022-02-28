using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos.Login
{
    public class LoginMobileDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
    }
}
