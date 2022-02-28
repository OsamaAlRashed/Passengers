using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using System;

namespace Passengers.Models.Main
{
    public class ShopCategory : BaseEntity
    {
        public Category Category { get; set; }
        public Guid CategoryId { get; set; }

        public Guid ShopId { get; set; }
        public AppUser Shop { get; set; }
    }
}
