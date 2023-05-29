﻿using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos
{
    public class SetAdminDto
    {
        public Guid Id { get; set; }
        public string ImageFile { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressText { get; set; }
        public GenderType? GenderType { get; set; }
        public decimal Salary { get; set; }
        public DateTime DOB { get; set; }
        public string Password { get; set; }
    }
}
