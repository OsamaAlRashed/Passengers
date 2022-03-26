using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class CustomerInformationDto
    {
        public string FullName { get; set; }
        public GenderTypes GenderType { get; set; }
        public DateTime DOB { get; set; }
    }
}
