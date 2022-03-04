using Passengers.Models.Base;
using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class Tag : BaseEntity
    {
        public Tag()
        {
            Products = new HashSet<Product>();
        }

        public string Name { get; set; }
        public string LogoPath { get; set; }
        public bool IsHidden { get; set; }

        public Guid? ShopId { get; set; }
        public AppUser Shop { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
