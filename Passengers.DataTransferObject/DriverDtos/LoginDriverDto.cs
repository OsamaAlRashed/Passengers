using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.DriverDtos
{
    public class LoginDriverDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
    }
}
