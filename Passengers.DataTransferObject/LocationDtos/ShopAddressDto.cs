﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.LocationDtos
{
    public class ShopAddressDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public Guid AreaId { get; set; }
        public Guid ShopId { get; set; }
    }
}
