using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Location
{
    public class Area : BaseEntity
    {
        public string Name { get; set; }

        public Guid CityId { get; set; }
        public City City { get; set; }

        public ICollection<Address> Addresses { get; set; }

    }
}
