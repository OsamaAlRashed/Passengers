using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class ShopContact : BaseEntity
    {
        public ContactShopTypes Type { get; set; }
        public string Text { get; set; }

        public Guid ShopId { get; set; }
        public AppUser Shop { get; set; }
    }
}
