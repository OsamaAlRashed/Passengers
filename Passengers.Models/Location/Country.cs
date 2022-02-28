using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Location
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }

}
