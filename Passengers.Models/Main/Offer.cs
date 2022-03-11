using Passengers.Models.Base;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class Offer : BaseEntity
    {
        public Offer()
        {
            Documents = new HashSet<Document>();
            OrderDetails = new HashSet<OrderDetails>();
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int PrepareTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public AppUser Shop { get; set; }
        public Guid ShopId { get; set; }

        public ICollection<Document> Documents { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }

        [NotMapped]
        public string ImagePath => Documents.Select(x => x.Path).FirstOrDefault();

        [NotMapped]
        public int Buyers => OrderDetails.Sum(x => x.Quantity);
    }
}
