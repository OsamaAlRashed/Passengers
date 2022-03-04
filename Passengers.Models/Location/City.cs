using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Location
{
    public class City : BaseEntity
    {
        public City()
        {
            Areas = new HashSet<Area>();
        }

        public string Name { get; set; }
        
        public Guid CountryId { get; set; }
        public Country Country { get; set; }

        public ICollection<Area> Areas { get; set; }
    }
}
