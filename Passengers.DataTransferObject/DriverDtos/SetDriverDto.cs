using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.DriverDtos
{
    public class SetDriverDto
    {
        public Guid Id { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressText { get; set; }
        public GenderTypes? GenderType { get; set; }
        public BloodTypes BloodType { get; set; }
        public decimal FixedAmount { get; set; }
        public DateTime DOB { get; set; }
    }
}
