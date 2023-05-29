using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.DriverDtos
{
    public class GetDriverDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressText { get; set; }
        public string UserName { get; set; }
        public GenderType? GenderType { get; set; }
        public BloodType? BloodType { get; set; }
        public int? Age { get; set; }
        public DateTime? DOB { get; set; }
        public string IdentifierImagePath { get; set; }
        public bool IsBlocked { get; set; }
        public bool Online { get; set; }
        public decimal FixedAmount { get; set; }
    }
}
