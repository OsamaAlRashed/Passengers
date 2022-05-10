using Passengers.DataTransferObject.RateDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ProductDtos
{
    public class GetProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PrepareTime { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public bool Avilable { get; set; }
        public Guid TagId { get; set; }
        public bool IsNew { get; set; }
        public bool HasDiscount { get; set; }
        public decimal? Discount { get; set; }
        public double Rate { get; set; }
    }
}
