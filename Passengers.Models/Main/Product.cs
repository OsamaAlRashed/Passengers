using Passengers.Models.Base;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Main
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int PrepareTime { get; set; }
        public bool Avilable { get; set; }
        public ProductTypes ProductType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Tag Tag { get; set; }
        public Guid TagId { get; set; }

        public ICollection<Document> Documents { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<PriceLog> PriceLogs { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
