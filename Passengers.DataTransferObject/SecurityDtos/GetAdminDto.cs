using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos
{
    public class GetAdminDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressText { get; set; }
        public string UserName { get; set; }
        public GenderTypes? GenderType { get; set; }
        public decimal Salary { get; set; }
        public int? Age { get; set; }
        public string IdentifierImagePath { get; set; }
        public bool IsBlocked { get; set; }

    }
}
