using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class CreateAccountCustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public GenderType Gender { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
