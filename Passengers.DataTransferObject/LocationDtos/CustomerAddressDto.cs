using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.LocationDtos
{
    public class CustomerAddressDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Building { get; set; }
        public string Text { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public bool IsCurrentLocation { get; set; }
    }
}
